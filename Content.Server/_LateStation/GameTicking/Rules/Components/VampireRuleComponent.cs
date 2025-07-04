using Robust.Shared.GameStates;
using Robust.Shared.Serialization;                      // For [DataDefinition]
using Content.Server.GameTicking.Rules.Components;     // For GameRuleComponent

namespace Content.Server._LateStation.GameTicking.Rules.Components
{
    [DataDefinition]
    [RegisterComponent]
    [Access(typeof(Content.Server._LateStation.GameTicking.Rules.VampireRuleSystem))]
    public sealed partial class VampireRuleComponent : Component
    {
        [DataField("matriarchCount")]
        public int MatriarchCount { get; set; } = 1;
    }
}
