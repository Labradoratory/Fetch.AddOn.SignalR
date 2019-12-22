﻿using System.Threading;
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
        private readonly IHubContext<THub> _hubContext;

        public SignalrOnUpdated(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override uint Priority => 0;

        public override Task ProcessAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            // TODO: Convert changes to JSON patch.
            var patch = (Operation[])null; // changes.ToJsonPatch();
            return _hubContext.UpdateAsync(typeof(TEntity).Name, package.Entity.EncodeKeys(), patch, cancellationToken);
        }
    }
}
