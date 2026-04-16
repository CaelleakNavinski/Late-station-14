using System;
using Robust.Shared.Serialization;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores baseline round configuration for the Vampire rule.
/// Turning, reversion, and ratio logic will be implemented in follow-up passes.
/// </summary>
[RegisterComponent, Access(typeof(VampireRuleSystem))]
public sealed partial class VampireRuleComponent : Component
{
    [DataField]
    public TimeSpan ConversionDelay = TimeSpan.FromSeconds(120);

    [DataField]
    public TimeSpan MatriarchDeathReversionDelay = TimeSpan.FromSeconds(90);

    [DataField]
    public float RequiredVampireRatio = 0.70f;

    [DataField]
    public float DrawDeadCrewRatio = 0.90f;
}
