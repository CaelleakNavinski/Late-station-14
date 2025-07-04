using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;        // for ProtoId<…>
using Robust.Shared.Serialization;     
using Robust.Shared.ViewVariables;
using Content.Shared.StatusIcon;       // for FactionIconPrototype
using Content.Shared._LateStation.Vampires.Systems;  // for SharedVampireSystem

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState] 
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class SharedVampireMatriarchComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("statusIcon")]
        [AutoNetworkedField]
        public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireMatriarchFaction";
    }
}
