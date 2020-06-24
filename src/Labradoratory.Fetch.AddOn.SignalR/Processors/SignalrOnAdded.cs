using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    // TODO: Might need to add a way to conver the entity type to a view.

    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> added via an <see cref="EntityHub{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THub">The type of the hub.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityAddedProcessor{TEntity}" />
    public class SignalrOnAdded<TEntity, THub> : SignalrNotificationProcessorBase<TEntity, THub, EntityAddedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly ISignalrAddDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">[Optional] A data transformer to apply before sending the added notification.</param>
        public SignalrOnAdded(
            IHubContext<THub> hubContext,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupNameTransformer groupNameTransformer = null,
            ISignalrAddDataTransformer<TEntity> dataTransformer = null)
            : base(hubContext, groupSelectors, groupNameTransformer)
        {
            _dataTransformer = dataTransformer;
        }

        protected override string Action => "add";

        protected override Task<object> GetDataAsync(EntityAddedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            if (_dataTransformer != null)
                return _dataTransformer.TransformAsync(package, cancellationToken);

            return Task.FromResult<object>(package.Entity);
        }
    }
}
