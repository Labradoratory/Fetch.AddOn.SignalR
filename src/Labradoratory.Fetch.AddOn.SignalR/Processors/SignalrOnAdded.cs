using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    // TODO: Might need to add a way to conver the entity type to a view.

    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> added via an <see cref="TMessageSender"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TMessageSender">The type of the message sender.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityAddedProcessor{TEntity}" />
    public class SignalrOnAdded<TEntity, TMessageSender> : SignalrNotificationProcessorBase<TEntity, TMessageSender, EntityAddedPackage<TEntity>>
        where TEntity : Entity
        where TMessageSender : ISignalrMessageSender
    {
        private readonly ISignalrAddDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">[Optional] A data transformer to apply before sending the added notification.</param>
        public SignalrOnAdded(
            TMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null,
            ISignalrAddDataTransformer<TEntity> dataTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer)
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

    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> added.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityAddedProcessor{TEntity}" />
    public class SignalrOnAdded<TEntity> : SignalrOnAdded<TEntity, ISignalrMessageSender>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">[Optional] A data transformer to apply before sending the added notification.</param>
        public SignalrOnAdded(
            ISignalrMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null,
            ISignalrAddDataTransformer<TEntity> dataTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer, dataTransformer)
        { }
    }
}
