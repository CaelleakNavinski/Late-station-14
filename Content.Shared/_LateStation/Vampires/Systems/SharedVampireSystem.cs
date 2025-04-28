using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shared stub for vampire logic (used by Access attributes).
    /// </summary>
    [Virtual]
    public sealed class SharedVampireSystem : EntitySystem
    {
        // Intentionally empty: shared logic can be added here.
    }
}
