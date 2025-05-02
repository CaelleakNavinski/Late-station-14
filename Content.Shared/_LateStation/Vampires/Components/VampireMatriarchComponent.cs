using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;


namespace Content.Shared._LateStation.Vampires.Components
{
    /// <summary>
    /// Marker for the Matriarch role; holds her pre‐upgrade health values.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    // Only the shared sync system needs read/write here:
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireMatriarchComponent : Component
    {
        /// <summary>Stored MaxHP before Matriarch buff.</summary>
        [ViewVariables(VVAccess.ReadOnly)]
        public float OriginalMaxHP { get; set; }

        /// <summary>Stored crit threshold before Matriarch buff.</summary>
        [ViewVariables(VVAccess.ReadOnly)]
        public float OriginalCritThreshold { get; set; }
    }
}