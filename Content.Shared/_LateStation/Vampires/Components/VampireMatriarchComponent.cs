using Robust.Shared.GameStates;

namespace Content.Shared.Vampire.Components
{
    /// <summary>
    /// Marker for the head vampire (Matriarch). Presence on an entity
    /// lets other systems detect “this is the Matriarch” and grant
    /// special abilities later.
    /// </summary>
    [RegisterComponent, NetworkedComponent]
    public sealed partial class VampireMatriarchComponent : Component
    {
        // Intentionally empty: fields for future buffs/abilities can go here.
    }
}
