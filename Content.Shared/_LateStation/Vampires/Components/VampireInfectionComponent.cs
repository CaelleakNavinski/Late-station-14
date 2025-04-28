using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    public sealed partial class VampireInfectionComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("timeLeft")]
        [AutoNetworkedField]
        public float TimeLeft { get; set; } = 120f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("maxTime")]
        [AutoNetworkedField]
        public float MaxTime { get; set; } = 180f;
    }
}
