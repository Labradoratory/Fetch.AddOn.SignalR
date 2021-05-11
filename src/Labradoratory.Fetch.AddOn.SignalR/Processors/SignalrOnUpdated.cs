using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Data;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Sends Signalr notifications on <see cref="Entity"/> updated via an <see cref="TMessageSender"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.EntityUpdatedProcessor{TEntity}" />
    public class SignalrOnUpdated<TEntity> : SignalrNotificationProcessorBase<TEntity, EntityUpdatedPackage<TEntity>>
        where TEntity : Entity
    {
        private readonly ISignalrUpdateDataTransformer<TEntity> _dataTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="messageSenderProvider">Provider for sender of SignalR messages.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="dataTransformer">A data transformer to apply before sending the added notification.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrOnUpdated(
            ISignalrMessageSenderProvider messageSenderProvider,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null,
            ISignalrUpdateDataTransformer<TEntity> dataTransformer = null)
            : base(messageSenderProvider, groupSelectors, groupNameTransformer)
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
}
