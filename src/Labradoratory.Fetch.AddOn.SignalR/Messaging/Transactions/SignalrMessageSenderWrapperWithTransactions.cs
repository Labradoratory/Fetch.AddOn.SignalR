using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions
{
    public class SignalrMessageSenderWrapperWithTransactions : ISignalrMessageSender
    {
        public SignalrMessageSenderWrapperWithTransactions(
            ISignalrMessageSender messageSender,
            SignalrMessagingTransactionManager transactionManager)
        {
            MessageSender = messageSender;
            TransactionManager = transactionManager;
        }

        protected ISignalrMessageSender MessageSender { get; }
        public SignalrMessagingTransactionManager TransactionManager { get; }

        public async Task SendAsync(SignalrGroup group, string method, object data, CancellationToken cancellationToken = default)
        {
            if (TransactionManager.Transaction != null)
            {
                // There is a transaction, so queue up to send on commit.
                TransactionManager.Transaction.OnCommit(() => MessageSender.SendAsync(group, method, data, cancellationToken));
            }
            else
            {
                // No transaction, so send right away.
                await MessageSender.SendAsync(group, method, data, cancellationToken);
            }
        }
    }
}
