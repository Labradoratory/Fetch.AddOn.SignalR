using System;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name with added prefixes.
    /// </summary>
    /// <example>
    /// For an entity named "Entity" and the prefix adds "Parent", this selector will return group "parent/entity".
    /// </example>
    public class EntityWithPrefixGroupSelector<TEntity> : CustomGroupWithPrefixGroupSelector<TEntity>
        where TEntity : Entity
    {
        public EntityWithPrefixGroupSelector(Func<BaseEntityDataPackage<TEntity>, object[]> addPrefix, bool useFullName = false)
            : base(SignalrGroup.Create(useFullName ? typeof(TEntity).FullName : typeof(TEntity).Name), addPrefix)
        { }
    }
}
