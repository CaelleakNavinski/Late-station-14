using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Shared._LateStation.Vampires.Components
{
    [Access(typeof(SharedVampireSystem))]
    [RegisterComponent, NetworkedComponent]
    public sealed partial class VampireBiteToggleComponent : Component
    {
        /// literally just a marker
    }
}