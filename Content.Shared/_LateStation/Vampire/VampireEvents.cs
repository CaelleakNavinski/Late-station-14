using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._LateStation.Vampire;

/// <summary>
/// Raised when a vampire with conversion permission targets someone with Converting Bite.
/// </summary>
public sealed partial class VampireBiteActionEvent : EntityTargetActionEvent
{
}

/// <summary>
/// Completion event for the 1-second converting bite doafter.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class VampireBiteDoAfterEvent : SimpleDoAfterEvent
{
}
