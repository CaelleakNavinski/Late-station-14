using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    [ComponentProtoName("ServerVampireInfection")]
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
        public float PreviousTimeLeft { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("finalStage")]
        public int FinalStage { get; set; }

        public float[] FinalThresholds { get; set; } = new float[] { 30f, 20f, 10f, 5f, 2f };
    }
}
