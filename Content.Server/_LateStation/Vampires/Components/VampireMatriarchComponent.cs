using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;
using Content.Server._LateStation.Vampires.Systems;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    // Grant both SharedVampireSystem (for sync) and VampireRoleSystem (for HP buff/restore) write access
    [Access(typeof(SharedVampireSystem), typeof(VampireRoleSystem))]
    public sealed partial class VampireMatriarchComponent : Component
    {
        // The two fields/properties are declared in the shared partial.
        // This server partial only applies the [Access] attribute.
    }
}
