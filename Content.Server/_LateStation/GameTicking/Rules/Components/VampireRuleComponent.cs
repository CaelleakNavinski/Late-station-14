namespace Content.Server._LateStation.GameTicking.Rules.Components;

/// <summary>
/// Marker component for the Vampire round rule.
/// Additional round-state fields can be added here later as the mode grows.
/// </summary>
[RegisterComponent, Access(typeof(VampireRuleSystem))]
public sealed partial class VampireRuleComponent : Component
{
}