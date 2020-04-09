using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR
{
    /// <summary>
    /// Defines the members that a transformer of <see cref="EntityAddedPackage{TEntity}"/> should implement.  
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ISignalrAddDataTransformer<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// Transforms <see cref="EntityAddedPackage{TEntity}"/> into the data that should be sent via 
        /// a SignalR entity added notification.
        /// </summary>
        /// <param name="package">The package containing the data to transform.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The data that should be sent with a SignalR entity added notification.</returns>
        Task<object> TransformAsync(EntityAddedPackage<TEntity> package, CancellationToken cancellationToken = default);
    }
}
