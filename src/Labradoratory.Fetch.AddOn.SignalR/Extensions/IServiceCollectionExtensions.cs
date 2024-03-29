﻿using System;
using Labradoratory.Fetch.AddOn.SignalR.DependencyInjection;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.AddOn.SignalR.Messaging.Transactions;
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
        public static IServiceCollection AddFetchSignalrDefaultMessageSenderAsHub<THub>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, HubContextMessageSender<THub>> initializer = null)
            where THub : Hub
        {
            if (initializer == null)
            {
                serviceCollection.AddTransient<HubContextMessageSender<THub>>();
                serviceCollection.AddTransient<ISignalrMessageSender, HubContextMessageSender<THub>>();
            }
            else
            {
                serviceCollection.AddTransient(initializer);
                serviceCollection.AddTransient<ISignalrMessageSender, HubContextMessageSender<THub>>(sp => sp.GetRequiredService<HubContextMessageSender<THub>>());
            }
            return serviceCollection.AddFetchSignalrTransactions();
        }

        /// <summary>
        /// Adds the fetch signalr default <see cref="ISignalrMessageSender"/>.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddFetchSignalrDefaultMessageSender<TSender>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, TSender> initializer = null)
            where TSender : class, ISignalrMessageSender
        {
            if (initializer == null)
            {
                serviceCollection.AddTransient<TSender>();
                serviceCollection.AddTransient<ISignalrMessageSender, TSender>();
            }
            else
            {
                serviceCollection.AddTransient(initializer);
                serviceCollection.AddTransient<ISignalrMessageSender, TSender>(sp => sp.GetRequiredService<TSender>());
            }
            return serviceCollection.AddFetchSignalrTransactions();
        }

        private static IServiceCollection AddFetchSignalrTransactions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISignalrMessageSenderProvider, SignalrMessageSenderProviderWithTransactions>();
            serviceCollection.AddScoped<SignalrMessagingTransactionManager>();
            serviceCollection.AddTransient<SignalrMessageSenderWrapperWithTransactions>();
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
            if (actions.HasFlag(SignalrProcessActions.Add))
                serviceCollection.AddTransient<IProcessor<EntityAddedPackage<TEntity>>, SignalrOnAdded<TEntity>>();

            if (actions.HasFlag(SignalrProcessActions.Delete))
                serviceCollection.AddTransient<IProcessor<EntityDeletedPackage<TEntity>>, SignalrOnDeleted<TEntity>>();

            if (actions.HasFlag(SignalrProcessActions.Update))
                serviceCollection.AddTransient<IProcessor<EntityUpdatedPackage<TEntity>>, SignalrOnUpdated<TEntity>>();

            return serviceCollection;
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
