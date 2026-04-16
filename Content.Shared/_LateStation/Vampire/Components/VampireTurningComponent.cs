using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Applied to living or critical victims currently turning into a vampire.
/// This is a body-state curse and may continue on a corpse.
/// </summary>
[RegisterComponent]
public sealed partial class VampireTurningComponent : Component
{
    /// <summary>
    /// Remaining conversion time.
    /// Starts at 120 seconds.
    /// </summary>
    [DataField]
    public TimeSpan Remaining = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Once-per-second processing cadence.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextTick = TimeSpan.Zero;

    /// <summary>
    /// The vampire who applied the curse.
    /// Used later for attribution and future interaction hooks.
    /// </summary>
    [DataField]
    public EntityUid? Source;

    /// <summary>
    /// Tracks the last ordered final warning line that has already fired.
    /// 0 means none have fired yet.
    /// </summary>
    [DataField]
    public int FinalWarningStage = 0;
}
