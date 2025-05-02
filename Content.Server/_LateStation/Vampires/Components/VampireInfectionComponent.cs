using Content.Server._LateStation.Vampires.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Systems;

namespace Content.Server._LateStation.Vampires.Components
{
    /// <summary>
    /// Grants write‐access on all auto‐networked fields so the server system
    /// can mutate TimeLeft, PopupAccumulator, etc.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    // Allow SharedVampireSystem (for syncing) and VampireInfectionSystem (for ticking)
    [Access(typeof(SharedVampireSystem), typeof(VampireInfectionSystem))]
    public sealed partial class VampireInfectionComponent : Component
    {
        // All fields are inherited from the shared partial; no new members here.
    }
}
