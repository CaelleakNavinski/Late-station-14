using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;                         // ← add this
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;      // ← add this so Access finds SharedVampireSystem

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("statusIcon")]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

        [DataField("vampStartSound")]
        public SoundSpecifier VampireStartSound { get; set; }
            = new SoundPathSpecifier("/Audio/Antag/vampire_start.ogg");

        public override bool SessionSpecific => true;
    }
}
