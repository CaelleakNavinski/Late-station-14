using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Client.Shared._LateStation.Vampires.Components;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [Access(typeof(Content.Server._LateStation.Vampires.Systems.VampireRoleSystem))]
    public sealed partial class VampireMatriarchComponent : SharedVampireMatriarchComponent
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("originalMaxHP")]
        public float OriginalMaxHP { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("originalCritThreshold")]
        public float OriginalCritThreshold { get; set; }
    }
}