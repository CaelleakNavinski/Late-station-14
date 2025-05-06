using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireMatriarchComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        public float OriginalMaxHP { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        public float OriginalCritThreshold { get; set; }
    }
}
