using System.Threading;
using System.Threading.Tasks;
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
    public class SignalrOnAdded<TEntity, THub> : IProcessor<EntityAddedPackage<TEntity>>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly string _name;
        private readonly IHubContext<THub> _hubContext;
        private readonly ISignalrGroupNameTransformer _groupNameTransformer;
        private readonly ISignalrAddDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnAdded{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        public SignalrOnAdded(
            IHubContext<THub> hubContext,
            ISignalrGroupNameTransformer groupNameTransformer = null,
            ISignalrAddDataTransformer<TEntity> dataTransformer = null)
            : this(typeof(TEntity).Name, hubContext, groupNameTransformer, dataTransformer)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="name">The name to use to identify the type in notifications.</param>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        public SignalrOnAdded(
            string name,
            IHubContext<THub> hubContext,
            ISignalrGroupNameTransformer groupNameTransformer = null,
            ISignalrAddDataTransformer<TEntity> dataTransformer = null)
        {
            _name = name;
            _hubContext = hubContext;
            _groupNameTransformer = groupNameTransformer;
            _dataTransformer = dataTransformer;
        }

        public uint Priority => 0;

        public async Task ProcessAsync(EntityAddedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var data = await _dataTransformer?.TransformAsync(package) ?? package.Entity;
            if (data == null)
                return;

            await _hubContext.AddAsync(_name, data, _groupNameTransformer, cancellationToken);
        }
    }
}
