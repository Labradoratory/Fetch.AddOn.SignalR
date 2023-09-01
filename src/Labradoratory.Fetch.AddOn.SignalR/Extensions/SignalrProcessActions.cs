using System;

namespace Labradoratory.Fetch.AddOn.SignalR.Extensions
{    
    /// <summary>
    /// Represents the <see cref="Entity"/> processing actions that SignalR can notify on.
    /// </summary>
    public enum SignalrProcessActions
    {
        /// <summary><see cref="Entity"/> added.</summary>
        Add = 1,

        /// <summary><see cref="Entity"/> deleted.</summary>
        Delete = 2,

        /// <summary><see cref="Entity"/> updated.</summary>
        Update = 4,

        /// <summary><see cref="Entity"/> added, deleted and updated.</summary>
        All = 7,
    }
}
