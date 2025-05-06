using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireSharedComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("statusIcon")]
        [AutoNetworkedField]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

        [DataField("vampStartSound")]
        public SoundSpecifier VampireStartSound { get; set; } =
            new SoundPathSpecifier("/Audio/Antag/vampire_start.ogg");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("biteActionPrototype")]
        [AutoNetworkedField]
        public string BiteActionPrototype { get; set; } = "ActionVampireBite";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("biteActionEntity"), AutoNetworkedField]
        public EntityUid? BiteActionEntity;

        public override bool SessionSpecific => true;
    }
}
