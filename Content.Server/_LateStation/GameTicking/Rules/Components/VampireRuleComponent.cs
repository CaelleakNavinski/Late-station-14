using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Server.GameTicking.Rules;

namespace Content.Server._LateStation.GameTicking.Rules.Components
{
    /// <summary>
    /// Prototype‑backed component for the Vampire round rule.
    /// Maps to the “VampireRule” entry in roundstart.yml.
    /// </summary>
    [RegisterComponent]
    [ComponentProtoName("VampireRule")]
    public sealed class VampireRuleComponent : RuleComponent
    {
        /// <summary>
        /// How many Vampire Matriarchs to spawn at round start.
        /// </summary>
        [DataField("matriarchCount")]
        public int MatriarchCount { get; set; } = 1;
    }
}
