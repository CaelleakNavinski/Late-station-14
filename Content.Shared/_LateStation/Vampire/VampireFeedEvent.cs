using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._LateStation.Vampire;

/// <summary>
/// Raised when a vampire targets an entity with Feed.
/// The server side will validate the target and begin the feeding DoAfter.
/// </summary>
public sealed partial class VampireFeedActionEvent : EntityTargetActionEvent
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
