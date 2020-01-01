using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
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
    public class SignalrOnUpdated<TEntity, THub> : EntityUpdatedProcessor<TEntity>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly string _name;
        private readonly IHubContext<THub> _hubContext;
        private readonly ISignalrUpdateDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the updated notification.</param>
        /// <remarks>The <typeparamref name="TEntity"/> name will be used in notifications.</remarks>
        public SignalrOnUpdated(IHubContext<THub> hubContext, ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
            : this(typeof(TEntity).Name, hubContext, dataTransformer)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="name">The name to use to identify the type in notifications.</param>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the updated notification.</param>
        public SignalrOnUpdated(string name, IHubContext<THub> hubContext, ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
        {
            _name = name;
            _hubContext = hubContext;
            _dataTransformer = dataTransformer;
        }

        public override uint Priority => 0;

        public override Task ProcessAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            // TODO: Convert changes to JSON patch.  This might be a seperate library.
            var patch = _dataTransformer?.Transform(package) ?? null; // package.Changes.ToJsonPatch();
            return _hubContext.UpdateAsync(_name, package.Entity.EncodeKeys(), patch, cancellationToken);
        }
    }
}
