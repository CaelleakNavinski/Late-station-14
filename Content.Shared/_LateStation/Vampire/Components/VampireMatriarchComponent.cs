using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Body marker for the vampire matriarch.
/// This is separate from the mind-role component so body-based systems
/// like faction icons can distinguish the matriarch from other vampires.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class VampireMatriarchComponent : Component
{
    /// <summary>
    /// The faction icon prototype displayed for the vampire matriarch.
    /// </summary>
    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireMatriarchFaction";

    public override bool SessionSpecific => true;
}
