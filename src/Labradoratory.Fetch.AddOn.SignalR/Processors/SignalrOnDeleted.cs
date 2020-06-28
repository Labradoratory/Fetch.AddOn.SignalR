using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> deleted via an <see cref="EntityHub{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THub">The type of the hub.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityDeletedProcessor{TEntity}" />
    public class SignalrOnDeleted<TEntity, THub> : SignalrNotificationProcessorBase<TEntity, THub, EntityDeletedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnDeleted(
            IHubContext<THub> hubContext,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null)
            : base(hubContext, groupSelectors, groupNameTransformer)
        { }

        protected override string Action => "delete";

        protected override Task<object> GetDataAsync(EntityDeletedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(package.Entity.GetKeys());
        }
    }
}
