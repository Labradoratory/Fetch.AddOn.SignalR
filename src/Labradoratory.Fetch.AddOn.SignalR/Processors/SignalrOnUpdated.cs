using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> updated via an <see cref="TMessageSender"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TMessageSender">The type of message sender.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityUpdatedProcessor{TEntity}" />
    public class SignalrOnUpdated<TEntity, TMessageSender> : SignalrNotificationProcessorBase<TEntity, TMessageSender, EntityUpdatedPackage<TEntity>>
        where TEntity : Entity
        where TMessageSender : ISignalrMessageSender
    {
        private readonly ISignalrUpdateDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnUpdated(
            TMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null,
            ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer)
        {
            _dataTransformer = dataTransformer;
        }

        protected override string Action => "update";

        protected override async Task<object> GetDataAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken)
        {
            if (_dataTransformer != null)
                return await _dataTransformer.TransformAsync(package, cancellationToken);

            var patch = package.Changes.ToJsonPatch();
            if (patch == null || patch.Length == 0)
                return null;

            return new UpdateData
            {
                Keys = package.Entity.GetKeys(),
                Patch = patch
            };
        }
    }

    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityUpdatedProcessor{TEntity}" />
    public class SignalrOnUpdated<TEntity> : SignalrOnUpdated<TEntity, ISignalrMessageSender>
        where TEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSender">Sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnUpdated(
            ISignalrMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null,
            ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer, dataTransformer)
        { }
    }
}
