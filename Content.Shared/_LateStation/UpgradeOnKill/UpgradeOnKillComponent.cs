using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared._LateStation.UpgradeOnKill
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class UpgradeOnKillComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("killCount")]
        [AutoNetworkedField]              // ← mark it networked
        public int KillCount = 0;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("threshold")]
        public int Threshold = 3;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("upgradePrototype")]
        public string UpgradePrototype = "";
    }
}
