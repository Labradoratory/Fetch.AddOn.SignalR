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
        private readonly IHubContext<THub> _hubContext;

        public SignalrOnDeleted(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override uint Priority => 0;

        public override Task ProcessAsync(EntityDeletedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            return _hubContext.DeleteAsync(typeof(TEntity).Name, package.Entity.EncodeKeys(), cancellationToken);
        }
    }
}
