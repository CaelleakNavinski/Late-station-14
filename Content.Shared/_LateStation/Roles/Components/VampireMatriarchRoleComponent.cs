using Robust.Shared.GameStates;

namespace Content.Shared._LateStation.Roles.Components;

/// <summary>
/// Added to mind role entities to tag that they are the vampire matriarch.
/// Tracks how many vampires this matriarch has sired.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class VampireMatriarchRoleComponent : BaseMindRoleComponent
{
    [DataField, AutoNetworkedField]
    public uint SiredCount;
}
