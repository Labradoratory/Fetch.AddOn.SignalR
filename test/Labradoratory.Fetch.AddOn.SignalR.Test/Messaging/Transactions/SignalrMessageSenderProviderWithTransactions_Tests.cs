using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Messaging.Transactions
{
    public class SignalrMessageSenderProviderWithTransactions_Tests
    {
        [Fact]
        public void Get_TransactionBasedSender()
        {
            var mockSender = new Mock<ISignalrMessageSender>(MockBehavior.Strict);
            mockSender
                .Setup(s => s.SendAsync(It.IsAny<SignalrGroup>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(sp => sp.GetService(It.Is<Type>(v => v == typeof(SignalrMessageSenderWrapperWithTransactions))))
                .Returns(new SignalrMessageSenderWrapperWithTransactions(mockSender.Object, new SignalrMessagingTransactionManager()));

            var subject = new SignalrMessageSenderProviderWithTransactions(mockServiceProvider.Object);
            var result = subject.Get();

            Assert.IsType<SignalrMessageSenderWrapperWithTransactions>(result);

            var expectedGroup = SignalrGroup.Create("Test");
            var expectedMethod = "Method";
            var expectedData = new object();
            var expectedCancellationToken = new CancellationToken();
            result.SendAsync(expectedGroup, expectedMethod, expectedData, expectedCancellationToken);

            mockSender.Verify(s => s.SendAsync(
                It.Is<SignalrGroup>(v => v == expectedGroup),
                It.Is<string>(v => v == expectedMethod),
                It.Is<object>(v => v == expectedData),
                It.Is<CancellationToken>(v => v == expectedCancellationToken)),
                Times.Once);
        }
    }
}
