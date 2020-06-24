using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name and <see cref="Entity.EncodeKeys"/>.
    /// </summary>
    /// <example>
    /// For an entity named "Entity", this selector will return groups "entity/{encodedKeys}".
    /// </example>
    public class EntityIdGroupSelector<T> : EntityGroupSelector<T>
        where T : Entity
    {
        public override Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<T> dataPackage, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            return Task.FromResult<IEnumerable<string>>(new[] { $"{name}/{dataPackage.Entity.EncodeKeys()}" });
        }
    }
}
