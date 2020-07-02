using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name.
    /// </summary>
    /// <example>
    /// If the name value is "Test", this selector will return group "test".
    /// </example>
    public class CustomGroupSelector<TEntity> : ISignalrGroupSelector<TEntity>
        where TEntity : Entity
    {
        public CustomGroupSelector(SignalrGroup group)
        {
            Group = group;
        }

        public SignalrGroup Group { get; }

        public virtual Task<IEnumerable<SignalrGroup>> GetGroupAsync(BaseEntityDataPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<SignalrGroup>>(new List<SignalrGroup> { Group });
        }
    }
}
