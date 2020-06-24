using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Processors;
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
    public class SignalrOnDeleted<TEntity, THub> : IProcessor<EntityDeletedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly IEnumerable<ISignalrGroupSelector<TEntity>> _groupSelectors;
        private readonly ISignalrGroupNameTransformer _groupNameTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnDeleted(
            IHubContext<THub> hubContext,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupNameTransformer groupNameTransformer = null)
        {
            _hubContext = hubContext;
            _groupSelectors = groupSelectors;
            _groupNameTransformer = groupNameTransformer;
        }

        /// <inheritdoc />
        public uint Priority => 0;

        /// <inheritdoc />
        public async Task ProcessAsync(EntityDeletedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            // NOTE: We could make this run in parallel, but there may be situations where an user
            // may not want that to happen.  We'd probably need a flag to turn off parallel or something.
            // Right now it just isn't worth doing.
            foreach (var selector in _groupSelectors)
            {
                foreach (var group in await selector.GetGroupAsync(package, cancellationToken))
                {
                    await _hubContext.DeleteAsync(group, package.Entity.EncodeKeys(), _groupNameTransformer, cancellationToken);
                }
            }
        }
    }
}
