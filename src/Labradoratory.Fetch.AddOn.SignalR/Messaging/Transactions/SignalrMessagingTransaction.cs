using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions
{
    public class SignalrMessagingTransaction : IAsyncDisposable
    {
        private int _count;
        private bool _rollback ;
        private readonly Queue<Func<Task>> _onCommit = new Queue<Func<Task>>();
        private readonly Queue<Func<Task>> _onRollback = new Queue<Func<Task>>();

        internal SignalrMessagingTransaction()
        { }

        internal void Begin()
        {
            _count++;
        }

        public bool IsCommitted => _count == 0 || _rollback;

        public bool WasRolledBack => _rollback;

        public async Task CommitAsync()
        {
            _count--;
            if(_count == 0 && !_rollback)
            {
                while(_onCommit.Count > 0)
                {
                    var action = _onCommit.Dequeue();
                    await action();
                }
            }
        }

        public async ValueTask RollbackAsync()
        {
            if (_rollback)
                return;

            _rollback = true;
            while (_onRollback.Count > 0)
            {
                var action = _onRollback.Dequeue();
                await action();
            }
        }

        public ValueTask DisposeAsync()
        {
            if (_count > 0)
                return RollbackAsync();

            return new ValueTask(Task.CompletedTask);
        }

        public void OnCommit(Func<Task> action)
        {
            if (IsCommitted || WasRolledBack)
                throw new InvalidOperationException("Transaction already complete.");

            _onCommit.Enqueue(action);
        }

        public void OnRollback(Func<Task> action)
        {
            if (IsCommitted || WasRolledBack)
                throw new InvalidOperationException("Transaction already complete.");

            _onRollback.Enqueue(action);
        }
    }
}
