using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name.
    /// </summary>
    /// <example>
    /// For an entity named "Entity", this selector will return group "entity".
    /// </example>
    public class EntityGroupSelector<TEntity> : CustomNameGroupSelector<TEntity>
        where TEntity : Entity
    {
        public EntityGroupSelector(bool useFullName = false)
            : base(useFullName ? typeof(TEntity).FullName : typeof(TEntity).Name)
        { }
    }
}
