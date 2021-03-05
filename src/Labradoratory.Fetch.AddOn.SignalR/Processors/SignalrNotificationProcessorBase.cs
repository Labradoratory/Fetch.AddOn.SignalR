using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Hubs;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Processors
{
    public abstract class SignalrNotificationProcessorBase<TEntity, THub, TPackage> : IProcessor<TPackage>
        where TEntity : Entity
        where THub : Hub, IEntityHub
        where TPackage : BaseEntityDataPackage<TEntity>
    {
        // TODO: Add the ability to assign custom stage.

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
            ISignalrGroupTransformer groupNameTransformer = null)
        {
            HubContext = hubContext;
            GroupSelectors = groupSelectors;
            GroupNameTransformer = groupNameTransformer;
        }

        public IStage Stage { get; } = new NumericPriorityStage(0);

        protected abstract string Action { get; }

        protected IHubContext<THub> HubContext { get; }
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

        protected virtual Task SendAsync(string group, string action, object data, CancellationToken cancellationToken)
        {
            return HubContext.Clients.Group(group).SendAsync(action, data, cancellationToken);
        }
    }
}
