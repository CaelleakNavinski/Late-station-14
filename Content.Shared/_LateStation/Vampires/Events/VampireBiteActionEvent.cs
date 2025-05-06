using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;   // ← add this so [Prototype] resolves
using Robust.Shared.Serialization;

namespace Content.Shared._LateStation.Vampires.Events
{
    [Serializable]
    [NetSerializable]
    [Prototype("VampireBiteAction")]
    public sealed partial class VampireBiteActionEvent : EntityTargetActionEvent
    {

    }
}