using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging
{
    /// <summary>
    /// Defines the members that a provider <see cref="ISignalrMessageSender"/> instances should implement.
    /// </summary>
    public interface ISignalrMessageSenderProvider
    {
        /// <summary>
        /// Gets the instance of <see cref="ISignalrMessageSender"/>
        /// </summary>
        /// <returns></returns>
        ISignalrMessageSender Get();
    }
}
