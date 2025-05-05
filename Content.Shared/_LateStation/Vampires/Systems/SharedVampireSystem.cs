using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampires.Systems
{
    /// <summary>
    /// Shared‐side system for Vampire components (network syncing, etc.).
    /// Exists solely so `[Access(typeof(SharedVampireSystem))]` resolves in the shared assembly.
    /// </summary>
    public abstract class SharedVampireSystem : EntitySystem
    {
        // No implementation needed; it just has to exist.
    }
}
