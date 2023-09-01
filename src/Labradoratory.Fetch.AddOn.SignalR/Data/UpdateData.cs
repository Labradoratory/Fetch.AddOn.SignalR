using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Labradoratory.Fetch.AddOn.SignalR.Data
{
    /// <summary>
    /// Represents an update to an entity in the form of a JSON patch.
    /// </summary>
    public class UpdateData
    {
        /// <summary>
        /// Gets or sets the <see cref="Entity"/>'s keys.
        /// </summary>
        public object[] Keys { get; set; }
        /// <summary>
        /// Gets or sets the JSON patch <see cref="Operation"/>s.
        /// </summary>
        public Operation[] Patch { get; set; }
    }
}
