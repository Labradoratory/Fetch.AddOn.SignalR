using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name with added prefixes.
    /// </summary>
    /// <example>
    /// For an entity named "Entity" and the prefix adds "Parent", this selector will return group "parent/entity".
    /// </example>
    public class EntityWithPrefixGroupSelector<TEntity> : ISignalrGroupSelector<TEntity>
        where TEntity : Entity
    {
        public EntityWithPrefixGroupSelector(params Func<BaseEntityDataPackage<TEntity>, string>[] addPrefixes)
        {
            AddPrefixes = addPrefixes;
        }

        public Func<BaseEntityDataPackage<TEntity>, string>[] AddPrefixes { get; }

        protected virtual string GetName()
        {
            return typeof(TEntity).Name.ToLower();
        }

        public virtual Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            var groups = new List<string>();
            if(AddPrefixes?.Length > 0)
            {
                groups.AddRange(AddPrefixes.Select(prefixer => $"{prefixer(package)}/{name}"));
            }

            return Task.FromResult<IEnumerable<string>>(groups);
        }
    }
}
