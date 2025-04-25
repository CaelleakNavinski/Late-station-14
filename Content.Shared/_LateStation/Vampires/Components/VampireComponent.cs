using Content.Shared.Antag;
using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.Vampire.Components;

/// <summary>
/// Marks an entity as a full vampire.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedVampireSystem))]
public sealed partial class VampireComponent : Component
{
    /// <summary>
    /// The status icon prototype displayed for vampires.
    /// </summary>
    /// [DataField, ViewVariables(VVAccess.ReadWrite)]
    /// public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

    /// <summary>
    /// Sound that plays **only to you** when you become a vampire.
    /// </summary>
    [DataField]
    public SoundSpecifier VampireStartSound { get; set; } = new SoundPathSpecifier("/Audio/_LateStation/Ambience/Antag/vamp_greeting.ogg");

    public override bool SessionSpecific => true;
}
