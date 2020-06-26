using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Processors
{
    public class SignalrOnDeleted_Tests
    {
        [Fact]
        public void Priority_Zero()
        {
            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            var subject = new SignalrOnDeleted<TestEntity, TestHub>(mockContext.Object, Enumerable.Empty<ISignalrGroupSelector<TestEntity>>());
            Assert.Equal(0u, subject.Priority);
        }

        [Fact]
        public async Task ProcessAsync_SendsToGroups()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedGroup1_2 = "MyGroup1.2";
            var expectedGroup2 = "SomeOtherGroup2";

            var expectedGroup1Delete = $"{expectedGroup1.ToLower()}/delete";
            var expectedGroup1_2Delete = $"{expectedGroup1_2.ToLower()}/delete";
            var expectedGroup2Delete = $"{expectedGroup2.ToLower()}/delete";

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

            var subject = new SignalrOnDeleted<TestEntity, TestHub>(mockContext.Object, mockSelectors);
            await subject.ProcessAsync(new EntityDeletedPackage<TestEntity>(expectedEntity), expectedToken);

            mockClients.Verify(c => c.Group(expectedGroup1.ToLower()), Times.Once);
            mockClients.Verify(c => c.Group(expectedGroup1_2.ToLower()), Times.Once);
            mockClients.Verify(c => c.Group(expectedGroup2.ToLower()), Times.Once);

            var matchKey = new Func<object[], string, bool>((v, key) =>
            {
                if (v.Length != 1)
                    return false;

                if(v[0] is object[] keys)
                    return keys.Length == 1 && Equals(keys[0], key);

                return false;
            });
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1Delete, It.Is<object[]>(v => matchKey(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1_2Delete, It.Is<object[]>(v => matchKey(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup2Delete, It.Is<object[]>(v => matchKey(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_TransformsGroup()
        {
            var expectedGroup1 = "MyGroup1";
            var expectedGroup1Add = $"{expectedGroup1.ToLower()}/delete";
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
            mockNameTransformer.Setup(t => t.TransformAsync(It.Is<string>(v => v == expectedGroup1), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTransformedGroup);

            var subject = new SignalrOnDeleted<TestEntity, TestHub>(mockContext.Object, mockSelectors, groupNameTransformer: mockNameTransformer.Object);
            await subject.ProcessAsync(new EntityDeletedPackage<TestEntity>(expectedEntity), expectedToken);
            
            var matchKey = new Func<object[], string, bool>((v, key) =>
            {
                if (v.Length != 1)
                    return false;

                if (v[0] is object[] keys)
                    return keys.Length == 1 && Equals(keys[0], key);

                return false;
            });
            mockClients.Verify(c => c.Group(expectedTransformedGroup), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedGroup1Add, It.Is<object[]>(v => matchKey(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
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
