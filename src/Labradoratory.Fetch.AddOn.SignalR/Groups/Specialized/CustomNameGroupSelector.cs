using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups
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
        private readonly string _name;

        public CustomNameGroupSelector(string name)
        {
            _name = name;
        }

        protected override string GetName()
        {
            return _name;
        }
    }
}
