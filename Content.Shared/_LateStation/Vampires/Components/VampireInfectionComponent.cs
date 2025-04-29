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

        // Tracks accumulated time between whisper popups
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("popupAccumulator")]
        public float PopupAccumulator { get; set; }

        // For detecting threshold crossings
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("previousTimeLeft")]
        public float PreviousTimeLeft { get; set; }

        // Which final‐stage message we've shown
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("finalStage")]
        public int FinalStage { get; set; }
    }
}
