using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Microsoft.AspNetCore.SignalR;

namespace Labradoratory.Fetch.AddOn.SignalR.Hubs
{
    // TODO: Should probably put this in a shared library.

    /// <summary>
    /// Defines a SignalR hub that is used for sending entity change notifications.
    /// </summary>
    /// <seealso cref="Hub" />
    public class EntityHub<T> : Hub<T>, IEntityHub
        where T : class
    {
        private readonly ISignalrGroupTransformer _groupNameTransformer;

        public EntityHub(ISignalrGroupTransformer groupNameTransformer)
        {
            _groupNameTransformer = groupNameTransformer;
        }

        /// <summary>
        /// Gets the <see cref="IGroupManager"/> that should be used for managing groups.
        /// </summary>
        /// <returns>The <see cref="IGroupManager"/>.</returns>
        /// <remarks>
        /// This allows for some customization / extension that SignalR is currently lacking.
        /// The default implmentation just return the <see cref="Hub.Groups"/> property.
        /// </remarks>
        protected virtual IGroupManager GetGroups() => Groups;

        // TODO: Define what a "path" is.  Should we use a special object instead of just a string?
        /// <summary>
        /// Subscribes to receive notifications regarding the entity or entities specified by the path.
        /// </summary>
        /// <param name="path">
        /// <para>The path representing the entity or entities to subscribe to.</para>
        /// <para>The path should start with the entity type name and can be followed by the Id of a specific instance.</para>
        /// <para>Path parts are separated by a <c>/</c> character.</para>
        /// <list type="bullet">
        /// <item>
        /// <term>{type}</term>
        /// <description>This path will receive change notifications for all instances of the specified type.</description>
        /// </item>
        /// <item>
        /// <term>{type}/{id}</term>
        /// <description>This path will receive change notifications the instance of specified type and Id.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <exception cref="ArgumentException">path</exception>
        public virtual async Task SubscribeEntity(List<object> groupParts)
        {
            await GetGroups().AddToGroupAsync(
                Context.ConnectionId, 
                await _groupNameTransformer.TransformIfPossibleAsync(SignalrGroup.Create(groupParts.ToArray())));
        }

        /// <summary>
        /// Unsubscribes from change notifications for the specified entity or entities.
        /// </summary>
        /// <param name="path">The path representing the entity or entities to unsubscribe from.</param>
        /// <remarks>
        /// Must have called the Subscribe method with the <paramref name="path"/> in order to unsubscribe.
        /// </remarks>
        public virtual async Task UnsubscribeEntity(List<object> groupParts)
        {
            await GetGroups().RemoveFromGroupAsync(
                Context.ConnectionId, 
                await _groupNameTransformer.TransformIfPossibleAsync(SignalrGroup.Create(groupParts.ToArray())));
        }
    }

    /// <summary>
    /// Defines members that an entity hub should implement.
    /// </summary>
    /// <remarks>
    /// For internal use.  This is basically used just to limit the <see cref="IHubContextExtensions"/>
    /// to a subset of hubs.
    /// </remarks>
    public interface IEntityHub
    {
        Task SubscribeEntity(List<object> groupParts);
        Task UnsubscribeEntity(List<object> groupParts);
    }
}
