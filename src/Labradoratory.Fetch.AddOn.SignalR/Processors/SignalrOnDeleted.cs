using System.Threading;
using System.Threading.Tasks;
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
    public class SignalrOnDeleted<TEntity, THub> : EntityDeletedProcessor<TEntity>
        where TEntity : Entity
        where THub : Hub, IEntityHub
    {
        private readonly string _name;
        private readonly IHubContext<THub> _hubContext;

        public SignalrOnDeleted(IHubContext<THub> hubContext)
            : this(typeof(Entity).Name, hubContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="name">The name to use to identify the type in notifications.</param>
        /// <param name="hubContext">The hub context.</param>
        public SignalrOnDeleted(string name, IHubContext<THub> hubContext)
        {
            _name = name;
            _hubContext = hubContext;
        }

        public override uint Priority => 0;

        public override Task ProcessAsync(EntityDeletedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            return _hubContext.DeleteAsync(_name, package.Entity.EncodeKeys(), cancellationToken);
        }
    }
}
