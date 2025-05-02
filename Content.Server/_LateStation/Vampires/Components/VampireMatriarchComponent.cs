using Content.Server._LateStation.Vampires.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Server._LateStation.Vampires.Components
{
    /// <summary>
    /// Grants write‐access so VampireRoleSystem can set and restore the Matriarch’s HP values.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    // Allow SharedVampireSystem (sync) and VampireRoleSystem (buff/restore)
    [Access(typeof(SharedVampireSystem), typeof(VampireRoleSystem))]
    public sealed partial class VampireMatriarchComponent : Component
    {
        /// <summary>Stored MaxHP before Matriarch buff.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalMaxHP { get; set; }

        /// <summary>Stored crit threshold before Matriarch buff.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalCritThreshold { get; set; }
    }
}