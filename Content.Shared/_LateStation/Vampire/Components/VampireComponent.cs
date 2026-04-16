using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Applied to completed vampires.
/// Tracks conversion permission and brood lineage.
/// </summary>
[RegisterComponent]
public sealed partial class VampireComponent : Component
{
    /// <summary>
    /// True for the Matriarch and Exarch-capable vampires.
    /// Ordinary vampires remain false.
    /// </summary>
    [DataField]
    public bool CanConvert = false;

    /// <summary>
    /// The body entity of the Matriarch this vampire belongs to.
    /// For the Matriarch herself, this should be their own body entity.
    /// </summary>
    [DataField]
    public EntityUid? Matriarch;

    /// <summary>
    /// Runtime action entity for the Converting Bite action.
    /// </summary>
    public EntityUid? BiteAction;
}
