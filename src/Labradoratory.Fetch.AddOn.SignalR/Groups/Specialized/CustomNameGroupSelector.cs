using System;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name.
    /// </summary>
    /// <example>
    /// If the name value is "Test", this selector will return group "test".
    /// </example>
    public class CustomNameGroupSelector<TEntity> : EntityGroupSelector<TEntity>
        where TEntity : Entity
    {
        public CustomNameGroupSelector(string name)
        {
            Name = name;
        }

        public string Name { get; }

        protected override string GetName()
        {
            return Name;
        }
    }
}
