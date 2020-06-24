using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name and <see cref="Entity.EncodeKeys"/>.
    /// </summary>
    /// <example>
    /// If the name value is "Test", this selector will return group "test/{id}".
    /// </example>
    public class CustomNameIdGroupSelector<T> : CustomNameGroupSelector<T>
        where T : Entity
    {
        public CustomNameIdGroupSelector(string name)
            : base(name)
        { }

        public override Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<T> dataPackage, CancellationToken cancellationToken = default)
        {
            var name = GetName();
            return Task.FromResult<IEnumerable<string>>(new[] { $"{name}/{dataPackage.Entity.EncodeKeys()}" });
        }
    }
}
