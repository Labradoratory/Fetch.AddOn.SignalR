using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The transformed group name.</returns>
        Task<string> TransformAsync(string groupName, CancellationToken cancellationToken = default);
    }

    public static class ISignalrGroupNameTransformerExtensions
    {
        /// <summary>
        /// Transforms the value if the <see cref="ISignalrGroupNameTransformer"/> is not null.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The transformed value or the original value if the <see cref="ISignalrGroupNameTransformer"/> is null.</returns>
        public static Task<string> TransformIfPossibleAsync(this ISignalrGroupNameTransformer transformer, string value, CancellationToken cancellationToken = default)
        {
            return transformer?.TransformAsync(value) ?? Task.FromResult(value);
        }
    }
}
