using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared._LateStation.Vampires.Components
{
    [RegisterComponent]
    [NetworkedComponent]
    public sealed partial class SharedVampireMatriarchComponent : Component
    {
        // marker only; buffs tracked server-side
    }
}
