using Robust.Shared.GameStates;

namespace Content.Shared.Vampire.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed class VampireBiteToggleComponent : Component
    {
        // This component has no data—its mere presence means “bite is armed.”
    }
}
