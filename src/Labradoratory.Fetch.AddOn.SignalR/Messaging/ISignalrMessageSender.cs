using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging
{
    /// <summary>
    /// Defines the members that a sender of SignalR messages should implement.
    /// </summary>
    public interface ISignalrMessageSender
    {
        Task SendAsync(SignalrGroup group, string method, object data, CancellationToken cancellationToken = default);
    }
}
