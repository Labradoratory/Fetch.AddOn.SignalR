using System;

namespace Labradoratory.Fetch.AddOn.SignalR
{
    /// <summary>
    /// Defines members used for transforming a Signalr group name.
    /// </summary>
    public interface ISignalrGroupNameTransformer
    {
        /// <summary>
        /// Transforms the specified group name.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>The transformed group name.</returns>
        string Transform(string groupName);
    }

    public static class ISignalrGroupNameTransformerExtensions
    {
        /// <summary>
        /// Transforms the value if the <see cref="ISignalrGroupNameTransformer"/> is not null.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        /// <param name="value">The value.</param>
        /// <returns>The transformed value or the original value if the <see cref="ISignalrGroupNameTransformer"/> is null.</returns>
        public static string TransformIfPossible(this ISignalrGroupNameTransformer transformer, string value)
        {
            return transformer?.Transform(value) ?? value;
        }
    }
}
