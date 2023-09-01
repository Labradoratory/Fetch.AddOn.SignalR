using System;
using Labradoratory.Fetch.AddOn.SignalR.Groups;
using Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.Extensions.DependencyInjection;

namespace Labradoratory.Fetch.AddOn.SignalR.DependencyInjection
{
    public class SignalrGroupSelectorRegistrar<TEntity> where TEntity : Entity
    {
        private readonly IServiceCollection _serviceCollection;

        public SignalrGroupSelectorRegistrar(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        /// <summary>
        /// Uses a notification group under the <see cref="Entity"/>'s name.
        /// This allows notifications for a type of entity.
        /// </summary>
        public void UseEntityGroup(bool useFullName = false)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityGroupSelector<TEntity>(useFullName));
        }

        /// <summary>
        /// Uses a notification group under the <see cref="Entity"/>'s name with added prefix.
        /// This allows notifications for a type of entity.
        /// </summary>
        /// <param name="addPrefix">Function to add prefix to the group names.  Should return the group prefix parts.</param>
        public void UseEntityGroupWithPrefix(Func<BaseEntityDataPackage<TEntity>, object[]> addPrefix, bool useFullName = false)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityWithPrefixGroupSelector<TEntity>(addPrefix, useFullName));
        }

        /// <summary>
        /// Uses a notification group under the <see cref="Entity"/>'s name and keys from <see cref="Entity.EncodeKeys"/>.
        /// This allows notifications for a specific entity instance.
        /// </summary>
        public void UseEntityGroupWithKeys(bool useFullName = false)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityKeyGroupSelector<TEntity>(useFullName));
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/>.
        /// This allows notifictions for a type of entity using a custom name.
        /// </summary>
        public void UseGroup(SignalrGroup group)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomGroupSelector<TEntity>(group));
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/> with added prefix.
        /// This allows notifictions for a type of entity using a custom name.
        /// </summary>
        public void UseGroupWithPrefix(SignalrGroup group, Func<BaseEntityDataPackage<TEntity>, object[]> addPrefix)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomGroupWithPrefixGroupSelector<TEntity>(group, addPrefix));
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/> and keys from <see cref="Entity.EncodeKeys"/>.
        /// This allows notifications for a specific entity instance using a custom name.
        /// </summary>
        public void UseGroupWithKeys(SignalrGroup group)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomGroupKeyGroupSelector<TEntity>(group));
        }

        /// <summary>
        /// Uses a transient, custom selector.
        /// </summary>
        /// <typeparam name="T">The type of selector to use.</typeparam>
        public void UseCustomSelector<TSelector>() where TSelector : class, ISignalrGroupSelector<TEntity>
        {
            _serviceCollection.AddTransient<ISignalrGroupSelector<TEntity>, TSelector>();
        }
    }
}
