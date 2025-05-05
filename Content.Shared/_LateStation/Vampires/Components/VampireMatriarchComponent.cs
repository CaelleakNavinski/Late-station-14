using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireMatriarchComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalMaxHP { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalCritThreshold { get; set; }
    }
}
