using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;
using Content.Server._LateStation.Vampires.Systems;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    [Access(typeof(SharedVampireSystem), typeof(VampireInfectionSystem))]
    public sealed partial class VampireInfectionComponent : Component 
    { 
        /// add later
    }
}
