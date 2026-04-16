using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Applied to completed vampires.
/// Tracks conversion permission, brood lineage, and blood resource state.
/// </summary>
[RegisterComponent]
public sealed partial class VampireComponent : Component
{
    [DataField]
    public bool CanConvert = false;

    [DataField]
    public EntityUid? Matriarch;

    [DataField]
    public float Blood = 0f;

    [DataField]
    public float MaxBlood = 100f;

    /// <summary>
    /// After 60 seconds without a successful feed, blood begins decaying.
    /// </summary>
    [DataField]
    public TimeSpan BloodDecayDelay = TimeSpan.FromSeconds(60);

    /// <summary>
    /// While decaying, lose 1 blood every 4 seconds.
    /// </summary>
    [DataField]
    public TimeSpan BloodDecayInterval = TimeSpan.FromSeconds(4);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan LastFeedTime = TimeSpan.Zero;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextBloodDecayTick = TimeSpan.Zero;

    public EntityUid? BiteAction;
    public EntityUid? FeedAction;
}
