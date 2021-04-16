using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Messaging;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    /// <summary>
    /// Base class for processors that send SignalR notifications.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TMessageSender">The type of the message sender.</typeparam>
    /// <typeparam name="TPackage">The type of the package.</typeparam>
    /// <seealso cref="Labradoratory.Fetch.Processors.IProcessor{TPackage}" />
    public abstract class SignalrNotificationProcessorBase<TEntity, TMessageSender, TPackage> : IProcessor<TPackage>
        where TEntity : Entity
        where TMessageSender : ISignalrMessageSender
        where TPackage : BaseEntityDataPackage<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrNotificationProcessorBase{TEntity, TMessageSender, TPackage}"/> class.
        /// </summary>
        /// <param name="messageSender">The message sender.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrNotificationProcessorBase(
            TMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null)
        {
            MessageSender = messageSender;
            GroupSelectors = groupSelectors;
            GroupNameTransformer = groupNameTransformer;
        }

        public virtual uint Priority => 0;

        protected abstract string Action { get; }

        protected TMessageSender MessageSender { get; }
        protected IEnumerable<ISignalrGroupSelector<TEntity>> GroupSelectors { get; }
        protected ISignalrGroupTransformer GroupNameTransformer { get; }

        public virtual async Task ProcessAsync(TPackage package, CancellationToken cancellationToken = default)
        {
            // NOTE: We could make this run in parallel, but there may be situations where an user
            // may not want that to happen.  We'd probably need a flag to turn off parallel or something.
            // Right now it just isn't worth doing.
            var groups = new HashSet<SignalrGroup>();
            foreach (var selector in GroupSelectors)
            {
                foreach (var group in await selector.GetGroupAsync(package, cancellationToken))
                {
                    groups.Add(group);
                }
            }

            // If there are no groups, no need to continue.
            if (groups.Count == 0)
                return;

            var data = await GetDataAsync(package, cancellationToken);
            if (data == null)
                return;

            foreach (var group in groups)
            {
                var action = group.Append(Action);
                var transformedGroup = await GroupNameTransformer.TransformIfPossibleAsync(group, cancellationToken);
                await SendAsync(transformedGroup, action, data, cancellationToken);
            }
        }

        protected abstract Task<object> GetDataAsync(TPackage package, CancellationToken cancellationToken);

        protected virtual Task SendAsync(SignalrGroup group, string action, object data, CancellationToken cancellationToken)
        {
            return MessageSender.SendAsync(group, action, data, cancellationToken);
        }
    }

    public abstract class SignalrNotificationProcessorBase<TEntity, TPackage> : SignalrNotificationProcessorBase<TEntity, ISignalrMessageSender, TPackage>
        where TEntity : Entity
        where TPackage : BaseEntityDataPackage<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrNotificationProcessorBase{TEntity, TPackage}"/> class.
        /// </summary>
        /// <param name="messageSender">The message sender.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        public SignalrNotificationProcessorBase(
            ISignalrMessageSender messageSender,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupTransformer groupNameTransformer = null)
            : base(messageSender, groupSelectors, groupNameTransformer)
        { }
    }
}
