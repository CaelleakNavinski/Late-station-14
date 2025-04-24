using Robust.Shared.Prototypes;

namespace Content.Shared.Objectives.Prototypes
{
    [Prototype("collectBloodObjective")]
    public sealed partial class CollectBloodObjectivePrototype : ObjectivePrototype
    {
        [DataField("targetAmount")]
        public float TargetAmount = 100f;
    }
}
