using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
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
    // Grant both the shared sync system and server‐side VampireInfectionSystem write access
    [Access(typeof(SharedVampireSystem), typeof(VampireInfectionSystem))]
    public sealed partial class VampireInfectionComponent : Component
    {
        // All properties (TimeLeft, PopupAccumulator, PreviousTimeLeft, FinalStage, etc.)
        // are declared in the shared partial.  This server partial exists solely to
        // attach the correct [Access] attribute in the server assembly.
    }
}
