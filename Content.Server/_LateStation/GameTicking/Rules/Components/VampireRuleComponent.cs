using Robust.Shared.GameStates;
using Content.Server.GameTicking.Rules.Components;

namespace Content.Server._LateStation.GameTicking.Rules.Components;

[RegisterComponent]
[Access(typeof(VampireRuleSystem))]
public sealed class VampireRuleComponent : Component
{
    [DataField("matriarchCount")]
    public int MatriarchCount { get; } = 1;
}
