using Content.Shared.Antag;
using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared._LateStation.Vampires.Components
{
    /// <summary>
    /// Marks an entity as a full vampire.
    /// Plays a sound on conversion and displays a faction icon.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    [Access(typeof(Content.Shared._LateStation.Vampires.Systems.SharedVampireSystem))]
    public sealed partial class VampireComponent : Component
    {
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

        [DataField]
        public SoundSpecifier VampireStartSound { get; set; }
            = new SoundPathSpecifier("/Audio/Antag/vamp_greeting.ogg");

        public override bool SessionSpecific => true;
    }
}
