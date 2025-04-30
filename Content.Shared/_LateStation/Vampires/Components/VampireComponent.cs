using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;                         // for ProtoId<>
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;      // for SharedVampireSystem

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(SharedVampireSystem), typeof(VampireRoleSystem))]
    public sealed partial class VampireComponent : Component
    {
        /// <summary>
        /// Status icon shown for vampires.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("statusIcon")]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

        /// <summary>
        /// Sound that plays when you become a vampire.
        /// </summary>
        [DataField("vampStartSound")]
        public SoundSpecifier VampireStartSound { get; set; }
            = new SoundPathSpecifier("/Audio/Antag/vampire_start.ogg");

        /// <summary>
        /// Prototype ID for the bite action to add on conversion.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("biteActionPrototype")]
        public string BiteActionPrototype { get; set; } = "ActionVampireBite";

        /// <summary>
        /// Stores the action entity created so we can remove it cleanly later.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("biteActionEntity")]
        [AutoNetworkedField]
        public EntityUid? BiteActionEntity;

        public override bool SessionSpecific => true;
    }
}
