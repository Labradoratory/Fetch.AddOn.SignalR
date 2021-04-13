using System;
using Labradoratory.Fetch.AddOn.SignalR.DependencyInjection;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.AddOn.SignalR.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="IServiceCollection"/> a little easier.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the fetch signalr default <see cref="ISignalrMessageSender"/> to a <see cref="Hub"/>.
        /// </summary>
        /// <typeparam name="THub">The type of the hub.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddFetchSignalrDefaultMessageSenderAsHub<THub>(this IServiceCollection serviceCollection) where THub : Hub
        {
            serviceCollection.AddTransient<ISignalrMessageSender, HubContextMessageSender<THub>>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the fetch signalr default <see cref="ISignalrMessageSender"/>.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddFetchSignalrDefaultMessageSender<TSender>(this IServiceCollection serviceCollection) where TSender : class, ISignalrMessageSender
        {
            serviceCollection.AddTransient<ISignalrMessageSender, TSender>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the fetch signalr processor to handle the requested <see cref="Entity"/> processing actions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TMessageSender">The type of the message sender.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="actions">The actions to notify on.</param>
        /// <returns>The <paramref name="serviceCollection"/> to calls can be chained.</returns>
        public static IServiceCollection AddFetchSignalrProcessor<TEntity, TMessageSender>(
            this IServiceCollection serviceCollection,
            SignalrProcessActions actions = SignalrProcessActions.All)
            where TEntity : Entity
            where TMessageSender : ISignalrMessageSender
        {
            if (actions.HasFlag(SignalrProcessActions.Add))
                serviceCollection.AddTransient<IProcessor<EntityAddedPackage<TEntity>>, SignalrOnAdded<TEntity, TMessageSender>>();

            if (actions.HasFlag(SignalrProcessActions.Delete))
                serviceCollection.AddTransient<IProcessor<EntityDeletedPackage<TEntity>>, SignalrOnDeleted<TEntity, TMessageSender>>();

            if (actions.HasFlag(SignalrProcessActions.Update))
                serviceCollection.AddTransient<IProcessor<EntityUpdatedPackage<TEntity>>, SignalrOnUpdated<TEntity, TMessageSender>>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds the fetch signalr processor to handle the requested <see cref="Entity"/> processing actions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="actions">The actions to notify on.</param>
        /// <returns>The <paramref name="serviceCollection"/> to calls can be chained.</returns>
        public static IServiceCollection AddFetchSignalrProcessor<TEntity>(
            this IServiceCollection serviceCollection,
            SignalrProcessActions actions = SignalrProcessActions.All)
            where TEntity : Entity
        {
            return serviceCollection.AddFetchSignalrProcessor<TEntity, ISignalrMessageSender>(actions);
        }

        public static IServiceCollection AddFetchSignalrGroupSelectors<TEntity>(
            this IServiceCollection serviceCollection,
            Action<SignalrGroupSelectorRegistrar<TEntity>> groupRegistrar)
            where TEntity : Entity
        {
            groupRegistrar(new SignalrGroupSelectorRegistrar<TEntity>(serviceCollection));
            return serviceCollection;
        }
    }
}
