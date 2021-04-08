using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Messaging
{
    public class HubContextMessageSender_Tests
    {
        [Fact]
        public async Task SendAsync_CallsHubContext()
        {
            var mockProxy = new Mock<IClientProxy>(MockBehavior.Strict);
            mockProxy.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>(MockBehavior.Strict);
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockProxy.Object);

            var mockContext = new Mock<IHubContext<TestHub>>(MockBehavior.Strict);
            mockContext.SetupGet(c => c.Clients).Returns(mockClients.Object);

            var expectedGroup = SignalrGroup.Create("This", "Is", "A", "Test", "Group");
            var expectedMethod = "MyMethod";
            var expectedData = new object();

            var subject = new HubContextMessageSender<TestHub>(mockContext.Object);
            await subject.SendAsync(expectedGroup, expectedMethod, expectedData, CancellationToken.None);

            mockClients.Verify(c => c.Group(expectedGroup), Times.Once);
            mockProxy.Verify(p => p.SendCoreAsync(expectedMethod, It.Is<object[]>(v => v.Length == 1 && ReferenceEquals(v[0], expectedData)), It.IsAny<CancellationToken>()), Times.Once);
        }

        public class TestHub : Hub
        {
        }
    }
}
