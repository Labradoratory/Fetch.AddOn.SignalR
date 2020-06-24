using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Processors;
using Labradoratory.Fetch.Extensions;
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
        /// Adds the fetch signalr processor to handle the requested <see cref="Entity"/> processing actions.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="THub">The type of the hub.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="notificationName">The name to use in the notifications.</param>
        /// <param name="actions">The actions to notify on.</param>
        /// <returns>The <paramref name="serviceCollection"/> to calls can be chained.</returns>
        public static IServiceCollection AddFetchSignalrProcessor<TEntity, THub>(
            this IServiceCollection serviceCollection,
            SignalrProcessActions actions = SignalrProcessActions.All)
            where TEntity : Entity
            where THub : Hub, IEntityHub
        {
            if (actions.HasFlag(SignalrProcessActions.Add))
                serviceCollection.AddTransient<IProcessor<EntityAddedPackage<TEntity>>>(
                    sp => new SignalrOnAdded<TEntity, THub>(
                        sp.GetRequiredService<IHubContext<THub>>(),
                        sp.GetServices<ISignalrGroupSelector<TEntity>>(),
                        sp.GetService<ISignalrGroupNameTransformer>(),
                        sp.GetService<ISignalrAddDataTransformer<TEntity>>()));

            if (actions.HasFlag(SignalrProcessActions.Delete))
                serviceCollection.AddTransient<IProcessor<EntityDeletedPackage<TEntity>>>(
                    sp => new SignalrOnDeleted<TEntity, THub>(
                        sp.GetRequiredService<IHubContext<THub>>(),
                        sp.GetServices<ISignalrGroupSelector<TEntity>>(),
                        sp.GetService<ISignalrGroupNameTransformer>()));

            if (actions.HasFlag(SignalrProcessActions.Update))
                serviceCollection.AddTransient<IProcessor<EntityUpdatedPackage<TEntity>>>(
                    sp => new SignalrOnUpdated<TEntity, THub>(
                        sp.GetRequiredService<IHubContext<THub>>(),
                        sp.GetServices<ISignalrGroupSelector<TEntity>>(),
                        sp.GetService<ISignalrGroupNameTransformer>(),
                        sp.GetService<ISignalrUpdateDataTransformer<TEntity>>()));

            return serviceCollection;
        }
    }
}
