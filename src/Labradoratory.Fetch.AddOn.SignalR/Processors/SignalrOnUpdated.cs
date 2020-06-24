using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> updated via an <see cref="EntityHub{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THub">The type of the hub.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityUpdatedProcessor{TEntity}" />
    public class SignalrOnUpdated<TEntity, THub> : IProcessor<EntityUpdatedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly IEnumerable<ISignalrGroupSelector<TEntity>> _groupSelectors;
        private readonly ISignalrGroupNameTransformer _groupNameTransformer;
        private readonly ISignalrUpdateDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        public SignalrOnUpdated(
            IHubContext<THub> hubContext,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupNameTransformer groupNameTransformer = null,
            ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
        {
            _hubContext = hubContext;
            _groupSelectors = groupSelectors;
            _groupNameTransformer = groupNameTransformer;
            _dataTransformer = dataTransformer;
        }

        /// <inheritdoc />
        public uint Priority => 0;

        /// <inheritdoc />
        public async Task ProcessAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var patch = await GetPatchAsync(package, cancellationToken);
            // If the patch is null or empty, there's no need to send the update.
            if (patch == null || patch.Length == 0)
                return;

            // NOTE: We could make this run in parallel, but there may be situations where an user
            // may not want that to happen.  We'd probably need a flag to turn off parallel or something.
            // Right now it just isn't worth doing.
            foreach (var selector in _groupSelectors)
            {
                foreach (var group in await selector.GetGroupAsync(package, cancellationToken))
                {
                    await _hubContext.UpdateAsync(group, package.Entity.EncodeKeys(), patch, _groupNameTransformer, cancellationToken);
                }
            }
        }

        private Task<Operation[]> GetPatchAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            if (_dataTransformer != null)
                return _dataTransformer.TransformAsync(package, cancellationToken);

            return Task.FromResult(package.Changes.ToJsonPatch());
        }
    }
}
