using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Objectives.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class CollectBloodObjectiveComponent : Component
    {
        [DataField("targetAmount", required: true)]
        public float TargetAmount = 100f;

        [DataField("currentAmount", customTypeSerializer: typeof(StrictPrototypeIdSerializer<float>))]
        public float CurrentAmount = 0f;
    }
}
