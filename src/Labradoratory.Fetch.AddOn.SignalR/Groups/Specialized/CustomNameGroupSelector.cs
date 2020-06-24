using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name.
    /// </summary>
    /// <example>
    /// If the name value is "Test", this selector will return group "test".
    /// </example>
    public class CustomNameGroupSelector<T> : EntityGroupSelector<T>
        where T : Entity
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
