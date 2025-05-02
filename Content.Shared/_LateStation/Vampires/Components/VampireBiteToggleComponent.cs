using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;

namespace Content.Shared._LateStation.Vampires.Components
{
    [Access(typeof(SharedVampireSystem))]
    [RegisterComponent, NetworkedComponent]
    public sealed partial class VampireBiteToggleComponent : Component
    {
    }
}