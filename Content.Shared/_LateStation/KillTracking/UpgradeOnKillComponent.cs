using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared._LateStation.KillTracking
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class UpgradeOnKillComponent : Component
    {
        /// <summary>
        /// Number of kills required before upgrading.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("threshold")]
        public int Threshold = 3;

        /// <summary>
        /// Prototype ID to spawn when threshold is reached.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("upgradePrototype")]
        public string UpgradePrototype = "";

        /// <summary>
        /// How many kills have been accrued so far.
        /// </summary>
        [ViewVariables(VVAccess.ReadOnly)]
        [DataField("killCount")]
        public int KillCount = 0;
    }
}
