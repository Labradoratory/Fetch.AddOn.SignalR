using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Messaging.Transactions
{
    public class SignalrMessageSenderWithTransactions_Tests
    {
        [Fact]
        public async Task SendAsync_DirectSendWhenNoTransaction()
        {
            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender
                .Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var transactionManager = new SignalrMessagingTransactionManager();

            var subject = new SignalrMessageSenderWrapperWithTransactions(mockSender.Object, transactionManager);

            var expectedGroup = SignalrGroup.Create("Test");
            var expectedMethod = "Method";
            var expectedData = new object();
            var expectedCancellationToken = new CancellationToken();
            await subject.SendAsync(expectedGroup, expectedMethod, expectedData, expectedCancellationToken);

            mockSender.Verify(s => s.SendAsync(
                It.Is<SignalrGroup>(v => v == expectedGroup),
                It.Is<string>(v => v == expectedMethod),
                It.Is<object>(v => v == expectedData),
                It.Is<CancellationToken>(v => v == expectedCancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task SendAsync_QueueSendWhenNoTransaction()
        {
            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender
                .Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var transactionManager = new SignalrMessagingTransactionManager();

            var subject = new SignalrMessageSenderWrapperWithTransactions(mockSender.Object, transactionManager);

            var expectedGroup = SignalrGroup.Create("Test");
            var expectedMethod = "Method";
            var expectedData = new object();
            var expectedCancellationToken = new CancellationToken();
            await using (var transaction = transactionManager.BeginTransaction())
            {
                await subject.SendAsync(expectedGroup, expectedMethod, expectedData, expectedCancellationToken);
                // Make sure it didn't send immediately.
                mockSender.Verify(s => s.SendAsync(
                    It.IsAny<SignalrGroup>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()),
                    Times.Never);

                await transaction.CommitAsync();
            }

            mockSender.Verify(s => s.SendAsync(
                It.IsAny<SignalrGroup>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
