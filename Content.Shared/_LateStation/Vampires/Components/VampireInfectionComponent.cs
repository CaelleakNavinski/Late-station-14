using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Vampire.Components
{
    /// <summary>
    /// Tracks a pending vampire conversion and syncs its countdown to all clients.
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class VampireInfectionComponent : Component
    {
        /// <summary>
        /// Remaining time (in seconds) until the victim flips to full vampirism.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("timeLeft"), AutoNetworkedField]
        public float TimeLeft { get; set; } = 120f;

        /// <summary>
        /// Upper limit for how long conversion can be delayed (in seconds).
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("maxTime"), AutoNetworkedField]
        public float MaxTime { get; set; } = 180f;
    }
}
