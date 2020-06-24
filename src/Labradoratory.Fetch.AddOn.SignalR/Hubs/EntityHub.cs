﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.Operations;
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
        public virtual async Task SubscribeEntity(string path)
        {
            await GetGroups().AddToGroupAsync(Context.ConnectionId, path.ToLower());
        }

        /// <summary>
        /// Unsubscribes from change notifications for the specified entity or entities.
        /// </summary>
        /// <param name="path">The path representing the entity or entities to unsubscribe from.</param>
        /// <remarks>
        /// Must have called the Subscribe method with the <paramref name="path"/> in order to unsubscribe.
        /// </remarks>
        public virtual async Task UnsubscribeEntity(string path)
        {
            await GetGroups().RemoveFromGroupAsync(Context.ConnectionId, path.ToLower());
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
        Task SubscribeEntity(string path);
        Task UnsubscribeEntity(string path);
    }

    /// <summary>
    /// Methods to make working with IHubContext a little easier.
    /// </summary>
    public static class IHubContextExtensions
    {
        /// <summary>
        /// Sends an entity added notification to the appropriate groups for the provided <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="THub">The type of the Hub used for notifications.  Must be a type of <see cref="EntityHub"/>.</typeparam>
        /// <param name="context">The context to use to send notifications.</param>
        /// <param name="group">The name of the group to send notification to.  This is case insensitive.</param>
        /// <param name="data">The data to send the add notification for.</param>
        /// <param name="groupNameTransformer">[Optional] A transform that can alter the group name.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The task.</returns>
        public static async Task AddAsync<THub>(this IHubContext<THub> context, string group, object data, ISignalrGroupNameTransformer groupTransformer = null, CancellationToken cancellationToken = default)
            where THub : Hub, IEntityHub
        {
            group = group.ToLower();
            await context.Clients.Group(await groupTransformer.TransformIfPossibleAsync(group)).SendAsync($"{group}/add", data, cancellationToken);
        }

        /// <summary>
        /// Sends an entity updated notification to the appropriate groups for the provided <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="THub">The type of the Hub used for notifications.  Must be a type of <see cref="EntityHub"/>.</typeparam>
        /// <param name="context">The context to use to send notifications.</param>
        /// <param name="group">The name of the group to send notifications to.  This is case insensitive.</param>
        /// <param name="encodedKeys">The <see cref="Entity.EncodeKeys"/> of the entity to notify about.</param>
        /// <param name="patch">The patch included in the update.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The task.</returns>
        public static async Task UpdateAsync<THub>(this IHubContext<THub> context, string group, string encodedKeys, Operation[] patch, ISignalrGroupNameTransformer groupTransformer = null, CancellationToken cancellationToken = default)
            where THub : Hub, IEntityHub
        {
            group = group.ToLower();
            await context.Clients.Group(await groupTransformer.TransformIfPossibleAsync(group)).SendAsync($"{group}/update", encodedKeys, patch, cancellationToken);
        }

        /// <summary>
        /// Sends an entity deleted notification to the appropriate groups for the provided <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="THub">The type of the Hub used for notifications.  Must be a type of <see cref="EntityHub"/>.</typeparam>
        /// <param name="context">The context to use to send notifications.</param>
        /// <param name="group">The name of the group to send notifications to.  This is case insensitive.</param>
        /// <param name="encodedKeys">The <see cref="Entity.EncodeKeys"/> of the entity to notify about.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The task.</returns>
        public static async Task DeleteAsync<THub>(this IHubContext<THub> context, string group, string encodedKeys, ISignalrGroupNameTransformer groupTransformer = null, CancellationToken cancellationToken = default)
            where THub : Hub, IEntityHub
        {
            group = group.ToLower();
            await context.Clients.Group(await groupTransformer.TransformIfPossibleAsync(group)).SendAsync($"{group}/delete", encodedKeys, cancellationToken);
        }
    }
}
