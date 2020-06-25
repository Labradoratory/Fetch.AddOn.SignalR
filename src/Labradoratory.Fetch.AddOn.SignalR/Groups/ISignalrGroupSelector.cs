using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors.DataPackages;

namespace Labradoratory.Fetch.AddOn.SignalR.Groups
{
    /// <summary>
    /// Defines members used for transforming a Signalr group name.
    /// </summary>
    public interface ISignalrGroupSelector<T> where T : Entity
    {
        /// <summary>
        /// Gets groups names to send notification to, for the provided data package.
        /// </summary>
        /// <param name="package">The <see cref="DataPackage"/> containing the entity to send notifications for.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The transformed group name.</returns>
        Task<IEnumerable<string>> GetGroupAsync(BaseEntityDataPackage<T> package, CancellationToken cancellationToken = default);
    }
}
