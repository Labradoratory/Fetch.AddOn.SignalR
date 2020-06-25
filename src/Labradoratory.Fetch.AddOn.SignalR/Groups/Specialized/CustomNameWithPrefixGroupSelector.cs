using System;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups.Specialized
{
    /// <summary>
    /// A simple group selector that sends notification to a group for the custom name with prefixes added.
    /// </summary>
    /// <example>
    /// If the name value is "Test" and prefix adds "Hello", this selector will return group "hello/test".
    /// </example>
    public class CustomNameWithPrefixGroupSelector<TEntity> : EntityWithPrefixGroupSelector<TEntity>
        where TEntity : Entity
    {
        public CustomNameWithPrefixGroupSelector(string name, params Func<BaseEntityDataPackage<TEntity>, string>[] addPrefixes)
            : base(addPrefixes)
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
