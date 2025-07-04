using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Server.GameTicking.Rules;

namespace Content.Server._LateStation.Vampires.Rules
{
    [RegisterComponent]
    [ComponentProtoName("VampireRule")]
    public sealed class VampireRuleComponent : RuleComponent
    {
        [DataField("matriarchCount")]
        public int MatriarchCount { get; } = 1;
    }
}
