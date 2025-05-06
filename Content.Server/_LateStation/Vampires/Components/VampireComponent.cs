using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.GameObjects;

namespace Content.Server._LateStation.Vampires.Components
{
    [RegisterComponent, ComponentProtoName("ServerVampire"), NetworkedComponent, AutoGenerateComponentState]
    [Access(typeof(Content.Shared._LateStation.Vampires.Systems.SharedVampireSystem), typeof(Content.Server._LateStation.Vampires.Systems.VampireRoleSystem))]
    public sealed partial class VampireComponent : Component
    { 
        // this is fine
    }
}
