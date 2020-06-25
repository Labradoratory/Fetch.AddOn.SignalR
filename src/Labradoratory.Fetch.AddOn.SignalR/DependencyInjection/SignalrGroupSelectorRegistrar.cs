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
        public void UseEntityGroup()
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityGroupSelector<TEntity>());
        }

        /// <summary>
        /// Uses a notification group under the <see cref="Entity"/>'s name with added prefixes.
        /// This allows notifications for a type of entity.
        /// </summary>
        /// <param name="addPrefixes">Functions to add prefixes to the group names.</param>
        public void UseEntityGroupWithPrefix(params Func<BaseEntityDataPackage<TEntity>, string>[] addPrefixes)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityWithPrefixGroupSelector<TEntity>(addPrefixes));
        }

        /// <summary>
        /// Uses a notification group under the <see cref="Entity"/>'s name and keys from <see cref="Entity.EncodeKeys"/>.
        /// This allows notifications for a specific entity instance.
        /// </summary>
        public void UseEntityGroupWithKeys()
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new EntityKeyGroupSelector<TEntity>());
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/>.
        /// This allows notifictions for a type of entity using a custom name.
        /// </summary>
        public void UseNamedGroup(string name)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomNameGroupSelector<TEntity>(name));
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/> with added prefixes.
        /// This allows notifictions for a type of entity using a custom name.
        /// </summary>
        public void UseNamedGroupWithPrefix(string name, params Func<BaseEntityDataPackage<TEntity>, string>[] addPrefixes)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomNameWithPrefixGroupSelector<TEntity>(name, addPrefixes));
        }

        /// <summary>
        /// Uses a notification group with the specified <paramref name="name"/> and keys from <see cref="Entity.EncodeKeys"/>.
        /// This allows notifications for a specific entity instance using a custom name.
        /// </summary>
        public void UseNamedGroupWithKeys(string name)
        {
            _serviceCollection.AddSingleton<ISignalrGroupSelector<TEntity>>(new CustomNameKeyGroupSelector<TEntity>(name));
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
