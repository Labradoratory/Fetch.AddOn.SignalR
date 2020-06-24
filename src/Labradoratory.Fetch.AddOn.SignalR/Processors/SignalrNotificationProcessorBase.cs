using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    public abstract class SignalrNotificationProcessorBase<TEntity, THub, TPackage> : IProcessor<TPackage>
        where TEntity : Entity
        where THub : Hub, IEntityHub
        where TPackage : BaseEntityDataPackage<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrOnUpdated{TEntity, THub}"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="groupSelectors">A collection of selctors that will be used to determine the groups to send notifications to.</param>
        /// <param name="groupNameTransformer">[Optional] A transformer to apply to the group name.</param>
        /// <param name="dataTransformer">[Optional] A data transformer to apply before sending the added notification.</param>
        public SignalrNotificationProcessorBase(
            IHubContext<THub> hubContext,
            IEnumerable<ISignalrGroupSelector<TEntity>> groupSelectors,
            ISignalrGroupNameTransformer groupNameTransformer = null)
        {
            HubContext = hubContext;
            GroupSelectors = groupSelectors;
            GroupNameTransformer = groupNameTransformer;
        }

        public virtual uint Priority => 0;

        protected abstract string Action { get; }

        protected IHubContext<THub> HubContext { get; }
        protected IEnumerable<ISignalrGroupSelector<TEntity>> GroupSelectors { get; }
        protected ISignalrGroupNameTransformer GroupNameTransformer { get; }

        public virtual async Task ProcessAsync(TPackage package, CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync(package, cancellationToken);
            if (data == null)
                return;

            // NOTE: We could make this run in parallel, but there may be situations where an user
            // may not want that to happen.  We'd probably need a flag to turn off parallel or something.
            // Right now it just isn't worth doing.
            foreach (var selector in GroupSelectors)
            {
                foreach (var group in await selector.GetGroupAsync(package, cancellationToken))
                {
                    var action = PrefixActionWithGroup(group);
                    var transformedGroup = await GroupNameTransformer.TransformIfPossibleAsync(group, cancellationToken);
                    await SendAsync(transformedGroup.ToLower(), action, data, cancellationToken);
                }
            }
        }

        protected abstract Task<object> GetDataAsync(TPackage package, CancellationToken cancellationToken);

        protected virtual Task SendAsync(string group, string action, object data, CancellationToken cancellationToken)
        {
            return HubContext.Clients.Group(group).SendAsync(action, data, cancellationToken);
        }

        protected virtual string PrefixActionWithGroup(string group)
        {
            return $"{group}/{Action}".ToLower();
        }
    }
}
