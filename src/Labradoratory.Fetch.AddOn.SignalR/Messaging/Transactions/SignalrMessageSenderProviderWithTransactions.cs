using System;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions
{
    public class SignalrMessageSenderProviderWithTransactions : ISignalrMessageSenderProvider
    {
        private ISignalrMessageSender _messageSender;

        public SignalrMessageSenderProviderWithTransactions(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }

        private ISignalrMessageSender MessageSender => _messageSender ??= ServiceProvider.GetRequiredService<SignalrMessageSenderWrapperWithTransactions>();

        public ISignalrMessageSender Get()
        {
            return MessageSender;
        }
    }
}
