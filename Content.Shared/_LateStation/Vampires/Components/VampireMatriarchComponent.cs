using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    [AutoGenerateComponentState]
    public sealed partial class VampireMatriarchComponent : Component
    {
        // Marker for the Matriarch role
    }
}
