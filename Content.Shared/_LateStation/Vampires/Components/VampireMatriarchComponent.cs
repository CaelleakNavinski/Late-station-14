using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [Access(typeof(SharedVampireSystem))]
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class VampireMatriarchComponent : Component
    {
        [ViewVariables(VVAccess.ReadOnly)]
        [AutoNetworkedField]
        public float OriginalMaxHP;

        [ViewVariables(VVAccess.ReadOnly)]
        [AutoNetworkedField]
        public float OriginalCritThreshold;
    }
}