using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Processors
{
    public class SignalrOnUpdated_Tests
    {
        [Fact]
        public void Priority_Zero()
        {
            var mockSenderProvider = new Mock<ISignalrMessageSenderProvider>(MockBehavior.Strict);
            var subject = new SignalrOnUpdated<TestEntity>(mockSenderProvider.Object, Enumerable.Empty<ISignalrGroupSelector<TestEntity>>());
            Assert.Equal(NumericPriorityStage.Zero, subject.Stage);
        }

        [Fact]
        public async Task ProcessAsync_SendsToGroups()
        {
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedGroup1_2 = SignalrGroup.Create("MyGroup1_2");
            var expectedGroup2 = SignalrGroup.Create("SomeOtherGroup2");

            var expectedAction = "update";
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedChanges = ChangeSet.Create(ChangePath.Empty);
            expectedChanges.Add(ChangePath.Create("test"), new List<ChangeValue> { new ChangeValue { Action = ChangeAction.Update, NewValue = "mynewvalue", OldValue = "myoldvalue" } });

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockSenderProvider = new Mock<ISignalrMessageSenderProvider>(MockBehavior.Strict);
            mockSenderProvider.Setup(p => p.Get()).Returns(mockSender.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1, expectedGroup1_2 });
            var mockSelector2 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector2.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup2 });
            var mockSelectors = new[] { mockSelector1.Object, mockSelector2.Object };

            var subject = new SignalrOnUpdated<TestEntity>(mockSenderProvider.Object, mockSelectors);
            await subject.ProcessAsync(new EntityUpdatedPackage<TestEntity>(expectedEntity, expectedChanges), expectedToken);

            mockSender.Verify(p => p.SendAsync(expectedGroup1, expectedGroup1.Append(expectedAction), It.Is<UpdateData>(v => CheckForPatch(v, expectedChanges, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(expectedGroup1_2, expectedGroup1_2.Append(expectedAction), It.Is<UpdateData>(v => CheckForPatch(v, expectedChanges, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(expectedGroup2, expectedGroup2.Append(expectedAction), It.Is<UpdateData>(v => CheckForPatch(v, expectedChanges, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        private bool CheckForPatch(UpdateData update, ChangeSet expectedChangeSet, string expectedKey)
        {
            Assert.Single(update.Keys);
            Assert.Equal(expectedKey, update.Keys[0]);

            var expectedPatch = expectedChangeSet.ToJsonPatch().ToList();
            var patch = update.Patch.ToList();

            while (expectedPatch.Count > 0 && patch.Count > 0)
            {
                var check = expectedPatch[0];
                expectedPatch.RemoveAt(0);
                var index = patch.FindIndex(p => p.path == check.path);
                if (index >= 0)
                    patch.RemoveAt(index);
            }

            return expectedPatch.Count == 0 && patch.Count == 0;
        }

        [Fact]
        public async Task ProcessAsync_TransformsData()
        {
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedAction = "update";
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedChanges = ChangeSet.Create(ChangePath.Empty);
            expectedChanges.Add(ChangePath.Create("test"), new List<ChangeValue> { new ChangeValue { Action = ChangeAction.Update, NewValue = "mynewvalue", OldValue = "myoldvalue" } });
            var expectedPackage = new EntityUpdatedPackage<TestEntity>(expectedEntity, expectedChanges);
            var expectedData = new UpdateData
            {
                Keys = expectedEntity.GetKeys(),
                Patch = expectedChanges.ToJsonPatch()
            };

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockSenderProvider = new Mock<ISignalrMessageSenderProvider>(MockBehavior.Strict);
            mockSenderProvider.Setup(p => p.Get()).Returns(mockSender.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockDataTransformer = new Mock<ISignalrUpdateDataTransformer<TestEntity>>(MockBehavior.Strict);
            mockDataTransformer.Setup(t => t.TransformAsync(It.IsAny<EntityUpdatedPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedData);

            var subject = new SignalrOnUpdated<TestEntity>(mockSenderProvider.Object, mockSelectors, dataTransformer: mockDataTransformer.Object);
            await subject.ProcessAsync(expectedPackage, expectedToken);

            mockDataTransformer.Verify(t => t.TransformAsync(expectedPackage, It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(expectedGroup1, expectedGroup1.Append(expectedAction), It.Is<UpdateData>(v => CheckForPatch(v, expectedChanges, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_DoesNotSendWhenTransformsDataNull()
        {
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);
            var expectedChanges = ChangeSet.Create(ChangePath.Empty);
            expectedChanges.Add(ChangePath.Create("test"), new List<ChangeValue> { new ChangeValue { Action = ChangeAction.Update, NewValue = "mynewvalue", OldValue = "myoldvalue" } });
            var expectedPackage = new EntityUpdatedPackage<TestEntity>(expectedEntity, expectedChanges);

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()));

            var mockSenderProvider = new Mock<ISignalrMessageSenderProvider>(MockBehavior.Strict);
            mockSenderProvider.Setup(p => p.Get()).Returns(mockSender.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockDataTransformer = new Mock<ISignalrUpdateDataTransformer<TestEntity>>(MockBehavior.Strict);
            mockDataTransformer.Setup(t => t.TransformAsync(It.IsAny<EntityUpdatedPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as UpdateData);

            var subject = new SignalrOnUpdated<TestEntity>(mockSenderProvider.Object, mockSelectors, dataTransformer: mockDataTransformer.Object);
            await subject.ProcessAsync(expectedPackage, expectedToken);

            mockDataTransformer.Verify(t => t.TransformAsync(expectedPackage, It.IsAny<CancellationToken>()), Times.Once);
            mockSender.Verify(p => p.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<UpdateData>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_TransformsGroup()
        {
            var expectedAction = "update";
            var expectedGroup1 = SignalrGroup.Create("MyGroup1");
            var expectedTransformedGroup = expectedGroup1.Prepend("transform");

            var expectedKey = "MyKey123";
            var expectedEntity = new TestEntity(expectedKey);

            var expectedChanges = ChangeSet.Create(ChangePath.Empty);
            expectedChanges.Add(ChangePath.Create("test"), new List<ChangeValue> { new ChangeValue { Action = ChangeAction.Update, NewValue = "mynewvalue", OldValue = "myoldvalue" } });

            var expectedToken = new CancellationToken();

            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender.Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockSenderProvider = new Mock<ISignalrMessageSenderProvider>(MockBehavior.Strict);
            mockSenderProvider.Setup(p => p.Get()).Returns(mockSender.Object);

            var mockSelector1 = new Mock<ISignalrGroupSelector<TestEntity>>(MockBehavior.Strict);
            mockSelector1.Setup(s => s.GetGroupAsync(It.IsAny<BaseEntityDataPackage<TestEntity>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { expectedGroup1 });
            var mockSelectors = new[] { mockSelector1.Object };

            var mockNameTransformer = new Mock<ISignalrGroupTransformer>(MockBehavior.Strict);
            mockNameTransformer.Setup(t => t.TransformAsync(It.Is<SignalrGroup>(v => v == expectedGroup1), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTransformedGroup);

            var subject = new SignalrOnUpdated<TestEntity>(mockSenderProvider.Object, mockSelectors, groupNameTransformer: mockNameTransformer.Object);
            await subject.ProcessAsync(new EntityUpdatedPackage<TestEntity>(expectedEntity, expectedChanges), expectedToken);

            mockSender.Verify(p => p.SendAsync(expectedTransformedGroup, expectedGroup1.Append(expectedAction), It.Is<UpdateData>(v => CheckForPatch(v, expectedChanges, expectedKey)), It.IsAny<CancellationToken>()), Times.Once);
        }

        public class TestHub : Hub, IEntityHub
        {
            public Task SubscribeEntity(List<object> groupParts)
            {
                throw new NotImplementedException();
            }

            public Task UnsubscribeEntity(List<object> groupParts)
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
