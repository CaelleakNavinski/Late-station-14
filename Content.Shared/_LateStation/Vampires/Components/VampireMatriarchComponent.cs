using Content.Server._LateStation.Vampires.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [Access(typeof(SharedVampireSystem), typeof(VampireRoleSystem))]
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class VampireMatriarchComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalMaxHP;

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float OriginalCritThreshold;
    }
}