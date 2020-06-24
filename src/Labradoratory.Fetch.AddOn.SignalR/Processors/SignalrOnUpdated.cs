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
    public class SignalrOnUpdated<TEntity, THub> : SignalrNotificationProcessorBase<TEntity, THub, EntityUpdatedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
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
            : base(hubContext, groupSelectors, groupNameTransformer)
        {
            _dataTransformer = dataTransformer;
        }

        protected override string Action => "update";

        protected override async Task<object> GetDataAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            var patch = await GetPatchAsync(package, cancellationToken);
            if (patch == null || patch.Length == 0)
                return null;

            return patch;
        }

        private Task<Operation[]> GetPatchAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            if (_dataTransformer != null)
                return _dataTransformer.TransformAsync(package, cancellationToken);

            return Task.FromResult(package.Changes.ToJsonPatch());
        }
    }
}
