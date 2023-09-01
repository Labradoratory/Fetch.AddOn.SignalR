using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the entity name and <see cref="Entity.EncodeKeys"/>.
    /// </summary>
    /// <example>
    /// For an entity named "Entity", this selector will return groups "entity/{encodedKeys}".
    /// </example>
    public class EntityKeyGroupSelector<TEntity> : CustomGroupKeyGroupSelector<TEntity>
        where TEntity : Entity
    {
        public EntityKeyGroupSelector(bool useFullName = false)
            : base(SignalrGroup.Create(useFullName ? typeof(TEntity).FullName : typeof(TEntity).Name))
        { }
    }
}
