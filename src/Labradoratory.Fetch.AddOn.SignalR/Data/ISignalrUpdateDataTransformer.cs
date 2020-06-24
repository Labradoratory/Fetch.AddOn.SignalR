using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Labradoratory.Fetch.AddOn.SignalR.Data
{
    // TODO: This works, but it feels kind of sloppy.  Maybe we should just let
    // the user send whatever they want as part of the notification.

    /// <summary>
    /// Defines the members that a transformer of <see cref="EntityUpdatedPackage{TEntity}"/> should implement.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ISignalrUpdateDataTransformer<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Transforms <see cref="EntityUpdatedPackage{TEntity}"/> into the data that should be sent via
        /// a SignalR entity added notification.
        /// </summary>
        /// <param name="package">The package containing the data to transform.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The data that should be sent with a SignalR entity added notification.</returns>
        Task<Operation[]> TransformAsync(EntityUpdatedPackage<TEntity> package, CancellationToken cancellationToken = default);
    }
}
