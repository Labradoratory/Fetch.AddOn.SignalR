using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Extensions;
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
        /// Adds the fetch signalr processor to handle the requested <see cref="Entity"/> processing actions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="THub">The type of the hub.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="actions">The actions to notify on.</param>
        /// <returns></returns>
        public static IServiceCollection AddFetchSignalrProcessor<TEntity, THub>(this IServiceCollection serviceCollection, SignalrProcessActions actions)
            where TEntity : Entity
            where THub : Hub, IEntityHub
        {
            if (actions.HasFlag(SignalrProcessActions.Add))
                serviceCollection.AddFetchAddedProcessor<TEntity, SignalrOnAdded<TEntity, THub>>();

            if (actions.HasFlag(SignalrProcessActions.Delete))
                serviceCollection.AddFetchDeletedProcessor<TEntity, SignalrOnDeleted<TEntity, THub>>();

            if (actions.HasFlag(SignalrProcessActions.Update))
                serviceCollection.AddFetchUpdatedProcessor<TEntity, SignalrOnUpdated<TEntity, THub>>();

            return serviceCollection;
        }
    }
}
