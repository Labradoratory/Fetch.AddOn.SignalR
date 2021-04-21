using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Processors
{
    public class SignalrOnDeleted_Tests
    {
        [Fact]
        public void Priority_Zero()
        {
            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            var subject = new SignalrOnDeleted<TestEntity>(mockSender.Object, Enumerable.Empty<ISignalrGroupSelector<TestEntity>>());
            Assert.Equal(NumericPriorityStage.Zero, subject.Stage);
        }

        [Fact]
        public async Task ProcessAsync_SendsToGroups()
        {
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedGroup1_2 = SignalrGroup.Create("MyGroup1_2");
            var expectedGroup2 = SignalrGroup.Create("SomeOtherGroup2");

            var expectedAction = "delete";
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1, expectedGroup1_2 });
            var mockSelector2 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector2.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup2 });
            var mockSelectors = new[] { mockSelector1.Object, mockSelector2.Object };

            var subject = new SignalrOnDeleted<TestEntity>(mockSender.Object, mockSelectors);
            await subject.ProcessAsync(new EntityDeletedPackage<TestEntity>(expectedEntity), expectedToken);

            mockSender.Verify(p => p.SendAsync(expectedGroup1, expectedGroup1.Append(expectedAction), It.Is<object[]>(v => IsKeyMatch(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(expectedGroup1_2, expectedGroup1_2.Append(expectedAction), It.Is<object[]>(v => IsKeyMatch(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(expectedGroup2, expectedGroup2.Append(expectedAction), It.Is<object[]>(v => IsKeyMatch(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_TransformsGroup()
        {
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedAction = "delete";
            var expectedTransformedGroup = expectedGroup1.Prepend("transformed");

            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockNameTransformer = new Mock<ISignalrGroupTransformer>(MockBehavior.Strict);
            mockNameTransformer.Setup(t => t.TransformAsync(It.Is<SignalrGroup>(v => v == expectedGroup1), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTransformedGroup);

            var subject = new SignalrOnDeleted<TestEntity>(mockSender.Object, mockSelectors, groupNameTransformer: mockNameTransformer.Object);
            await subject.ProcessAsync(new EntityDeletedPackage<TestEntity>(expectedEntity), expectedToken);
            
            mockSender.Verify(s => s.SendAsync(expectedTransformedGroup, expectedGroup1.Append(expectedAction), It.Is<object[]>(v => IsKeyMatch(v, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        private bool IsKeyMatch(object[] keys, object expectedKey)
        {
            return keys.Length == 1 && Equals(keys[0], expectedKey);
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
