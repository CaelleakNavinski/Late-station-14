using System;
using Content.Shared.Radio;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Applied to completed vampires.
/// Tracks conversion permission, brood lineage, and blood resource state.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class VampireComponent : Component
{
    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "VampireFaction";

    public override bool SessionSpecific => true;

    [DataField]
    public bool IsExarch = false;

    [DataField]
    public EntityUid? Matriarch;

    [DataField]
    [AutoNetworkedField]
    public float Blood = 30f;

    [DataField]
    public float MaxBlood = 200f;

    /// <summary>
    /// The percentage of the victim's current bloodstream volume removed on each completed feed cycle.
    /// Example: 0.02 = 2% of their current blood.
    /// </summary>
    [DataField]
    public float FeedTargetBloodDrainFraction = 0.02f;

    /// <summary>
    /// How efficiently extracted victim blood becomes vampire blood.
    /// Example: 0.33 means draining 15 victim blood yields about 5 vampire blood.
    /// </summary>
    [DataField]
    public float FeedEfficiency = 0.33f;

    /// <summary>
    /// Blood cost to activate Bloodsprint.
    /// </summary>
    [DataField]
    public float BloodSprintCost = 12f;

    /// <summary>
    /// Duration of Bloodsprint.
    /// </summary>
    [DataField]
    public TimeSpan BloodSprintDuration = TimeSpan.FromSeconds(6);

    /// <summary>
    /// Walk speed modifier while Bloodsprint is active.
    /// </summary>
    [DataField]
    public float BloodSprintWalkSpeedModifier = 1.22f;

    /// <summary>
    /// Sprint speed modifier while Bloodsprint is active.
    /// </summary>
    [DataField]
    public float BloodSprintSprintSpeedModifier = 1.30f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan BloodSprintEndTime = TimeSpan.Zero;

    /// <summary>
    /// Blood cost to activate Mist Form.
    /// </summary>
    [DataField]
    public float MistFormCost = 18f;

    /// <summary>
    /// The radio channel intrinsically granted to vampires.
    /// This channel's dedicated keycode is (:r), but is also set to the default radio (:h).
    /// </summary>
    [DataField]
    public ProtoId<RadioChannelPrototype> RadioChannel = "Vampire";

    /// <summary>
    /// Tracks which channels this component added to ActiveRadioComponent.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannels = new();

    /// <summary>
    /// Tracks which channels this component added to IntrinsicRadioTransmitterComponent.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannels = new();

    public EntityUid? BiteAction;
    public EntityUid? FeedAction;
    public EntityUid? BloodSprintAction;
    public EntityUid? MistFormAction;
}
