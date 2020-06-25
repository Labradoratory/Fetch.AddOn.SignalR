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
        protected virtual string GetName()
        {
            return typeof(TEntity).Name.ToLower();
        }

        public virtual Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            return Task.FromResult<IEnumerable<string>>(new List<string> { name });
        }
    }
}
