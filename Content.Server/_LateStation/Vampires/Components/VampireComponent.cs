using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;
using Client.Shared._LateStation.Vampires.Components;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    [Access(typeof(Content.Shared._LateStation.Vampires.Systems.SharedVampireSystem), typeof(Content.Server._LateStation.Vampires.Systems.VampireRoleSystem))]
    public sealed partial class VampireComponent : SharedVampireComponent
    { 
        // this is fine
    }
}
