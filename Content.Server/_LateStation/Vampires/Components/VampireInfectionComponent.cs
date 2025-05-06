using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(Content.Server._LateStation.Vampires.Systems.VampireInfectionSystem))]
    public sealed partial class VampireInfectionComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("timeLeft")]
        [AutoNetworkedField]
        public float TimeLeft { get; set; } = 120f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("maxTime")]
        public float MaxTime { get; set; } = 180f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("popupAccumulator")]
        [AutoNetworkedField]
        public float PopupAccumulator { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("previousTimeLeft")]
        [AutoNetworkedField]
        public float PreviousTimeLeft { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("finalStage")]
        [AutoNetworkedField]
        public int FinalStage { get; set; }
    }
}
