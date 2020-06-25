using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name.
    /// </summary>
    /// <example>
    /// For an entity named "Entity", this selector will return group "entity".
    /// </example>
    public class EntityGroupSelector<TEntity> : ISignalrGroupSelector<TEntity>
        where TEntity : Entity
    {
        private readonly Func<BaseEntityDataPackage<TEntity>, string>[] _addPrefixes;

        public EntityGroupSelector(params Func<BaseEntityDataPackage<TEntity>, string>[] addPrefixes)
        {
            _addPrefixes = addPrefixes;
        }

        protected virtual string GetName()
        {
            return typeof(TEntity).Name.ToLower();
        }

        public virtual Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            var groups = new List<string>();
            if(_addPrefixes?.Length > 0)
            {
                groups.AddRange(_addPrefixes.Select(prefixer => $"{prefixer(package)}/{name}"));
            }
            else
            {
                groups.Add(name);
            }

            return Task.FromResult<IEnumerable<string>>(groups);
        }
    }
}
