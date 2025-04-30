using Content.Shared.Roles;

namespace Content.Server.Roles;

/// <summary>
///     Added to mind role entities to tag that they are a Vampire.
/// </summary>
[RegisterComponent]
public sealed partial class VampireRoleComponent : BaseMindRoleComponent
{
}
