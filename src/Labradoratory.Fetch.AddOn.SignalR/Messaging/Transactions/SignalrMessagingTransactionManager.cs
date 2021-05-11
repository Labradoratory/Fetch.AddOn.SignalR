using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions
{
    public class SignalrMessagingTransactionManager
    {
        internal SignalrMessagingTransaction Transaction { get; private set; }

        public SignalrMessagingTransaction BeginTransaction()
        {
            if (Transaction == null)
                Transaction = new SignalrMessagingTransaction();

            Transaction.Begin();
            return Transaction;
        }
    }
}
