using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> deleted via an <see cref="TMessageSender"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TMessageSender">The type of message sender.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityDeletedProcessor{TEntity}" />
    public class SignalrOnDeleted<TEntity, TMessageSender> : SignalrNotificationProcessorBase<TEntity, TMessageSender, EntityDeletedPackage<TEntity>>
        where TEntity : Entity
        where TMessageSender : ISignalrMessageSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnDeleted(
            TMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer)
        { }

        protected override string Action => "delete";

        protected override Task<object> GetDataAsync(EntityDeletedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(package.Entity.GetKeys());
        }
    }

    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> deleted.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THub">The type of the hub.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityDeletedProcessor{TEntity}" />
    public class SignalrOnDeleted<TEntity> : SignalrOnDeleted<TEntity, ISignalrMessageSender>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnDeleted(
            ISignalrMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer)
        { }
    }
}
