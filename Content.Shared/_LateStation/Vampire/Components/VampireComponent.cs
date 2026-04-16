using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampire.Components;

/// <summary>
/// Applied to completed vampires.
/// This slice only tracks whether the vampire may use Converting Bite.
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
    /// Runtime action entity for the Converting Bite action.
    /// </summary>
    public EntityUid? BiteAction;
}
