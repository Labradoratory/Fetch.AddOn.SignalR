using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.AddOn.SignalR.Groups;

namespace Labradoratory.Fetch.AddOn.SignalR
{
    /// <summary>
    /// Defines members used for transforming a Signalr group.
    /// </summary>
    public interface ISignalrGroupTransformer
    {
        /// <summary>
        /// Transforms the specified group.
        /// </summary>
        /// <param name="group">The group to transform.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The transformed group name.</returns>
        Task<SignalrGroup> TransformAsync(SignalrGroup group, CancellationToken cancellationToken = default);
    }

    public static class ISignalrGroupNameTransformerExtensions
    {
        /// <summary>
        /// Transforms the value if the <see cref="ISignalrGroupTransformer"/> is not null.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        /// <param name="value">The value.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The transformed value or the original value if the <see cref="ISignalrGroupTransformer"/> is null.</returns>
        public static Task<SignalrGroup> TransformIfPossibleAsync(this ISignalrGroupTransformer transformer, SignalrGroup value, CancellationToken cancellationToken = default)
        {
            return transformer?.TransformAsync(value, cancellationToken) ?? Task.FromResult(value);
        }
    }
}
