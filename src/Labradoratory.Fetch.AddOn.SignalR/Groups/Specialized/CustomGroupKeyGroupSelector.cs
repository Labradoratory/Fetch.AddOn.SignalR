using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name and <see cref="Entity.EncodeKeys"/>.
    /// </summary>
    /// <example>
    /// If the name value is "Test", this selector will return group "test/{id}".
    /// </example>
    public class CustomGroupKeyGroupSelector<T> : CustomGroupSelector<T>
        where T : Entity
    {
        public CustomGroupKeyGroupSelector(SignalrGroup group)
            : base(group)
        { }

        public override Task<IEnumerable<SignalrGroup>> GetGroupAsync(BaseEntityDataPackage<T> dataPackage, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<SignalrGroup>>(new[] { Group.Append(dataPackage.Entity.GetKeys()) });
        }
    }
}
