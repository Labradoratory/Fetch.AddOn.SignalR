using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Processors
{
    public class SignalrOnAdded_Tests
    {
        [Fact]
        public void Priority_Zero()
        {
            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            var subject = new SignalrOnAdded<TestEntity, TestHub>(mockContext.Object, Enumerable.Empty<ISignalrGroupSelector<TestEntity>>());
            Assert.Equal(0u, subject.Priority);
        }
        
        [Fact]
        public async Task ProcessAsync_SendsToGroups()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedGroup1_2 = "MyGroup1.2";
            var expectedGroup2 = "SomeOtherGroup2";

            var expectedGroup1Add = $"{expectedGroup1.ToLower()}/add";
            var expectedGroup1_2Add = $"{expectedGroup1_2.ToLower()}/add";
            var expectedGroup2Add = $"{expectedGroup2.ToLower()}/add";

            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedToken = new CancellationToken();

            var mockProxy = new Mock<IClientProxy>(MockBehavior.Strict);
            mockProxy.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>(MockBehavior.Strict);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockProxy.Object);

            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            mockContext.SetupGet(c => c.Clients).Returns(mockClients.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1, expectedGroup1_2 });
            var mockSelector2 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector2.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup2 });
            var mockSelectors = new[] { mockSelector1.Object, mockSelector2.Object };

            var subject = new SignalrOnAdded<TestEntity, TestHub>(mockContext.Object, mockSelectors);
            await subject.ProcessAsync(new EntityAddedPackage<TestEntity>(expectedEntity), expectedToken);

            mockClients.Verify(c => c.Group(expectedGroup1.ToLower()), Times.Once);
            mockClients.Verify(c => c.Group(expectedGroup1_2.ToLower()), Times.Once);
            mockClients.Verify(c => c.Group(expectedGroup2.ToLower()), Times.Once);

            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1Add, It.Is<object[]>(v => v.Contains(expectedEntity)), It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1_2Add, It.Is<object[]>(v => v.Contains(expectedEntity)), It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup2Add, It.Is<object[]>(v => v.Contains(expectedEntity)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_TransformsData()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);
            var expectedPackage = new EntityAddedPackage<TestEntity>(expectedEntity);

            var expectedData = new object();

            var expectedToken = new CancellationToken();

            var mockProxy = new Mock<IClientProxy>(MockBehavior.Strict);
            mockProxy.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>(MockBehavior.Strict);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockProxy.Object);

            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            mockContext.SetupGet(c => c.Clients).Returns(mockClients.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockDataTransformer = new Mock<ISignalrAddDataTransformer<TestEntity>>(MockBehavior.Strict);
            mockDataTransformer.Setup(t => t.TransformAsync(It.IsAny<EntityAddedPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedData);

            var subject = new SignalrOnAdded<TestEntity, TestHub>(mockContext.Object, mockSelectors, dataTransformer: mockDataTransformer.Object);
            await subject.ProcessAsync(expectedPackage, expectedToken);

            mockDataTransformer.Verify(t => t.TransformAsync(expectedPackage, It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(It.IsAny<string>(), It.Is<object[]>(v => v.Contains(expectedData)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_DoesNotSendWhenTransformsDataNull()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);
            var expectedPackage = new EntityAddedPackage<TestEntity>(expectedEntity);

            var expectedToken = new CancellationToken();

            var mockProxy = new Mock<IClientProxy>(MockBehavior.Strict);
            mockProxy.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>(MockBehavior.Strict);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockProxy.Object);

            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            mockContext.SetupGet(c => c.Clients).Returns(mockClients.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockDataTransformer = new Mock<ISignalrAddDataTransformer<TestEntity>>(MockBehavior.Strict);
            mockDataTransformer.Setup(t => t.TransformAsync(It.IsAny<EntityAddedPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(null);

            var subject = new SignalrOnAdded<TestEntity, TestHub>(mockContext.Object, mockSelectors, dataTransformer: mockDataTransformer.Object);
            await subject.ProcessAsync(expectedPackage, expectedToken);

            mockDataTransformer.Verify(t => t.TransformAsync(expectedPackage, It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_TransformsGroup()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedGroup1Add = $"{expectedGroup1.ToLower()}/add";
            var expectedTransformedGroup = $"transformed.{expectedGroup1}".ToLower();

            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedToken = new CancellationToken();

            var mockProxy = new Mock<IClientProxy>(MockBehavior.Strict);
            mockProxy.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>(MockBehavior.Strict);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockProxy.Object);

            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            mockContext.SetupGet(c => c.Clients).Returns(mockClients.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockNameTransformer = new Mock<ISignalrGroupNameTransformer>(MockBehavior.Strict);
            mockNameTransformer.Setup(t => t.TransformAsync(It.Is<string>(v => v == expectedGroup1.ToLower()), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTransformedGroup);

            var subject = new SignalrOnAdded<TestEntity, TestHub>(mockContext.Object, mockSelectors, groupNameTransformer: mockNameTransformer.Object);
            await subject.ProcessAsync(new EntityAddedPackage<TestEntity>(expectedEntity), expectedToken);

            mockClients.Verify(c => c.Group(expectedTransformedGroup), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1Add, It.Is<object[]>(v => v.Contains(expectedEntity)), It.IsAny<CancellationToken>()), Times.Once);
        }

        public class TestHub : Hub, IEntityHub
        {
            public Task SubscribeEntity(string path)
            {
                throw new NotImplementedException();
            }

            public Task UnsubscribeEntity(string path)
            {
                throw new NotImplementedException();
            }
        }

        public class TestEntity : Entity
        {
            private readonly string _key;

            public TestEntity(string key)
            {
                _key = key;
            }

            public override object[] DecodeKeys(string encodedKeys)
            {
                throw new NotImplementedException();
            }

            public override string EncodeKeys()
            {
                return _key;
            }

            public override object[] GetKeys()
            {
                return ToKeys(_key);
            }
        }
    }
}
