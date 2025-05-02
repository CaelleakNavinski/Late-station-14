using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    // Only the shared system needs read/write here
    [Access(typeof(SharedVampireSystem))]
    public sealed partial class VampireInfectionComponent : Component
    {
        /// <summary>Seconds remaining until full conversion.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("timeLeft")]
        [AutoNetworkedField]
        public float TimeLeft { get; set; } = 120f;

        /// <summary>Maximum possible conversion time.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("maxTime")]
        [AutoNetworkedField]
        public float MaxTime { get; set; } = 180f;

        /// <summary>Accumulator for whisper‐popup timing.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("popupAccumulator")]
        [AutoNetworkedField]
        public float PopupAccumulator { get; set; }

        /// <summary>Stores previous tick’s TimeLeft for threshold checks.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("previousTimeLeft")]
        [AutoNetworkedField]
        public float PreviousTimeLeft { get; set; }

        /// <summary>Index of the last “final stage” message shown.</summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("finalStage")]
        [AutoNetworkedField]
        public int FinalStage { get; set; }
    }
}