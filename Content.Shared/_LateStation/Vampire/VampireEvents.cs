using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._LateStation.Vampire;

/// <summary>
/// Raised when a vampire with sire-tier permission targets someone with Converting Bite.
/// </summary>
public sealed partial class VampireBiteActionEvent : EntityTargetActionEvent
{
}

/// <summary>
/// Raised when a vampire targets an entity with Feed.
/// The server side will validate the target and begin the feeding DoAfter.
/// </summary>
public sealed partial class VampireFeedActionEvent : EntityTargetActionEvent
{
}

/// <summary>
/// Raised when a vampire activates Bloodsprint.
/// </summary>
public sealed partial class VampireBloodSprintActionEvent : InstantActionEvent
{
}

/// <summary>
/// Raised when a vampire activates Mist Form.
/// </summary>
public sealed partial class VampireMistFormActionEvent : InstantActionEvent
{
}

/// <summary>
/// Completion event for the 1-second converting bite doafter.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class VampireBiteDoAfterEvent : SimpleDoAfterEvent
{
}

/// <summary>
/// Completion event for a single feeding cycle.
/// The server side may choose to restart feeding again if conditions still hold.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class VampireFeedDoAfterEvent : SimpleDoAfterEvent
{
}