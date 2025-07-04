using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Content.Server.GameTicking.Rules.Components;

namespace Content.Server._LateStation.GameTicking.Rules.Components;

[RegisterComponent]
[DataDefinition]
[Access(typeof(VampireRuleSystem))]
public sealed class VampireRuleComponent : Component
{
    [DataField("matriarchCount")]
    public int MatriarchCount { get; set; } = 1;
}
