using System;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Messaging.Transactions
{
    public class SignalrMessagingTransactionManager_Tests
    {
        [Fact]
        public async Task BeginTransaction_SameTransactionWhenNested()
        {
            var subject = new SignalrMessagingTransactionManager();

            await using (var transaction1 = subject.BeginTransaction())
            {
                await using(var transaction2 = subject.BeginTransaction())
                {
                    Assert.Same(transaction1, transaction2);
                }
            }
        }

        [Fact]
        public async Task Dispose_RollbackWhenNotCommitted()
        {
            var subject = new SignalrMessagingTransactionManager();

            var transaction1 = subject.BeginTransaction();
            await using (transaction1)
            {
            }

            Assert.True(transaction1.WasRolledBack);
        }

        [Fact]
        public async Task Dispose_NotRollbackWhenCommitted()
        {
            var subject = new SignalrMessagingTransactionManager();

            var transaction1 = subject.BeginTransaction();
            await using (transaction1)
            {
                await transaction1.CommitAsync();
            }

            Assert.False(transaction1.WasRolledBack);
        }
    }
}
