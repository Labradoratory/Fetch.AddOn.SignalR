using System.Collections.Generic;
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
    public class EntityGroupSelector<T> : ISignalrGroupSelector<T>
        where T : Entity
    {
        protected virtual string GetName()
        {
            return typeof(T).Name.ToLower();
        }

        public virtual Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<T> dataPackage, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            return Task.FromResult<IEnumerable<string>>(new[] { name });
        }
    }
}
