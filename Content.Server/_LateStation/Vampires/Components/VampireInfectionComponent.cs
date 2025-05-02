using Content.Server._LateStation.Vampires.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    // Allow both the shared sync system and the server infection system to write these fields
    [Access(typeof(SharedVampireSystem), typeof(VampireInfectionSystem))]
    public sealed partial class VampireInfectionComponent : Component
    {

    }
}
