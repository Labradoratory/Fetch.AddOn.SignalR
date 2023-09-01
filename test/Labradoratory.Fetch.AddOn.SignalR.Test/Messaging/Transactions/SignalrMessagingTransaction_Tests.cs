using System;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions;
using Xunit;

namespace Labradoratory.Fetch.AddOn.SignalR.Test.Messaging.Transactions
{
    public class SignalrMessagingTransaction_Tests
    {
        [Fact]
        public async Task Commit_DoesCallOnCommitWhenComplete()
        {
            var expectedActionCalled = false;
            var expectedAction = new Func<Task>(() =>
            {
                expectedActionCalled = true;
                return Task.CompletedTask;
            });

            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            
            subject.OnCommit(expectedAction);
            await subject.CommitAsync();

            Assert.True(subject.IsCommitted);
            Assert.False(subject.WasRolledBack);
            Assert.True(expectedActionCalled);
        }

        [Fact]
        public async Task Commit_DoesNotCallOnCommitWhenNestedAndNotComplete()
        {
            var expectedActionCalled = false;
            var expectedAction = new Func<Task>(() =>
            {
                expectedActionCalled = true;
                return Task.CompletedTask;
            });

            var subject = new SignalrMessagingTransaction();
            subject.Begin(); 
            subject.Begin();

            subject.OnCommit(expectedAction);
            await subject.CommitAsync();

            Assert.False(subject.IsCommitted);
            Assert.False(subject.WasRolledBack);
            Assert.False(expectedActionCalled);
        }

        [Fact]
        public async Task Commit_DoesCallOnCommitWhenNestedAndComplete()
        {
            var expectedActionCalled = false;
            var expectedAction = new Func<Task>(() =>
            {
                expectedActionCalled = true;
                return Task.CompletedTask;
            });

            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            subject.Begin();

            subject.OnCommit(expectedAction);
            await subject.CommitAsync();
            await subject.CommitAsync();

            Assert.True(subject.IsCommitted);
            Assert.False(subject.WasRolledBack);
            Assert.True(expectedActionCalled);
        }

        [Fact]
        public async Task OnCommit_ThrowsWhenCommitted()
        {
            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            await subject.CommitAsync();

            Assert.Throws<InvalidOperationException>(() => subject.OnCommit(() => Task.CompletedTask));
        }

        [Fact]
        public async Task OnCommit_ThrowsWhenRolledBack()
        {
            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            await subject.RollbackAsync();

            Assert.Throws<InvalidOperationException>(() => subject.OnCommit(() => Task.CompletedTask));
        }

        [Fact]
        public async Task Rollback_DoesCallOnRollback()
        {
            var expectedActionCalled = false;
            var expectedAction = new Func<Task>(() =>
            {
                expectedActionCalled = true;
                return Task.CompletedTask;
            });

            var subject = new SignalrMessagingTransaction();
            subject.Begin();

            subject.OnRollback(expectedAction);
            await subject.RollbackAsync();

            Assert.True(subject.IsCommitted);
            Assert.True(subject.WasRolledBack);
            Assert.True(expectedActionCalled);
        }

        [Fact]
        public async Task OnRollback_ThrowsWhenCommitted()
        {
            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            await subject.CommitAsync();

            Assert.Throws<InvalidOperationException>(() => subject.OnRollback(() => Task.CompletedTask));
        }

        [Fact]
        public async Task OnRollback_ThrowsWhenRolledBack()
        {
            var subject = new SignalrMessagingTransaction();
            subject.Begin();
            await subject.RollbackAsync();

            Assert.Throws<InvalidOperationException>(() => subject.OnRollback(() => Task.CompletedTask));
        }
    }
}
