using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name with prefixes added.
    /// </summary>
    /// <example>
    /// If the name value is "Test" and prefix adds "Hello", this selector will return group "hello/test".
    /// </example>
    public class CustomNameWithPrefixGroupSelector<TEntity> : CustomNameGroupSelector<TEntity>
        where TEntity : Entity
    {
        public CustomNameWithPrefixGroupSelector(Func<BaseEntityDataPackage<TEntity>, object[]> addPrefix, params object[] nameParts)
            : base(nameParts)
        {
            AddPrefix = addPrefix;
        }

        public Func<BaseEntityDataPackage<TEntity>, object[]> AddPrefix { get; }

        public override async Task<IEnumerable<SignalrGroup>> GetGroupAsync(BaseEntityDataPackage<TEntity> package, CancellationToken cancellationToken = default)
        {
            var group = await base.GetGroupAsync(package, cancellationToken);
            return group.Select(g => g.Prepend(AddPrefix(package)));
        }
    }
}
