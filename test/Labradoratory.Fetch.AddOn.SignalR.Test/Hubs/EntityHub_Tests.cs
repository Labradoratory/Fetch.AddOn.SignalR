using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Hubs
{
    public class EntityHub_Tests
    {
        [Fact]
        public void GetGroups_ReturnsBaseGroups()
        {
            var subject = new TestEntityHub(null, null);
            Assert.Same(subject.Groups, subject.TestGetGroups());
        }

        [Fact]
        public async Task SubscribeEntity_Success()
        {
            var expectedConnectionId = "connectionid";
            var expectedPath = new[] { "This", "Is", "My", "Path" };

            var mockGroupManager = new Mock<IGroupManager>(MockBehavior.Strict);
            mockGroupManager.Setup(g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var subject = new TestEntityHub(mockGroupManager.Object, expectedConnectionId);
            await subject.SubscribeEntity(expectedPath);

            mockGroupManager.Verify(g => g.AddToGroupAsync(expectedConnectionId, SignalrGroup.Create(expectedPath), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeEntity_Success()
        {
            var expectedConnectionId = "connectionid";
            var expectedPath = new[] { "This", "Is", "My", "Path" };

            var mockGroupManager = new Mock<IGroupManager>(MockBehavior.Strict);
            mockGroupManager.Setup(g => g.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var subject = new TestEntityHub(mockGroupManager.Object, expectedConnectionId);
            await subject.UnsubscribeEntity(expectedPath);

            mockGroupManager.Verify(g => g.RemoveFromGroupAsync(expectedConnectionId, SignalrGroup.Create(expectedPath), It.IsAny<CancellationToken>()), Times.Once);
        }

        public interface Test
        {}

        public class TestEntityHub : EntityHub<Test>
        {
            private readonly IGroupManager _groupManager;

            public TestEntityHub(IGroupManager groupManager, string connectionId)
                : base(null)
            {
                _groupManager = groupManager;
                var mockContext = new Mock<HubCallerContext>(MockBehavior.Strict);
                mockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);
                Context = mockContext.Object;
            }

            protected override IGroupManager GetGroups()
            {
                return _groupManager;
            }

            public IGroupManager TestGetGroups()
            {
                return base.GetGroups();
            }
        }
    }
}
