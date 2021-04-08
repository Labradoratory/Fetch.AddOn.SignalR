using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging
{
    /// <summary>
    /// An implementation of <see cref="ISignalrMessageSender"/> that uses an <see cref="IHubContext{THub}"/> to send the message to a group.
    /// </summary>
    /// <typeparam name="THub">The type of the hub.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.AddOn.SignalR.Messaging.ISignalrMessageSender" />
    public class HubContextMessageSender<THub> : ISignalrMessageSender
        where THub : Hub
    {
        public HubContextMessageSender(IHubContext<THub> context)
        {
            Context = context;
        }

        private IHubContext<THub> Context { get; }

        public Task SendAsync(SignalrGroup group, string method, object data, CancellationToken cancellationToken = default)
        {
            return Context.Clients.Group(group).SendAsync(method, data, cancellationToken);
        }
    }
}
