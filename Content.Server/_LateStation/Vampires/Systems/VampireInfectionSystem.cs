using Content.Shared.StatusEffect;               // StatusEffectsSystem
using Content.Shared.Popups;                     // SharedPopupSystem
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.GameStates;                  // EntitySystem
using Robust.Shared.Timing;                      // frameTime
using Robust.Shared.GameObjects;                 // EntityQuery
using Robust.Shared.IoC;                         // [Dependency]
using Robust.Shared.IoC;                         // [Dependency]

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireInfectionSystem : EntitySystem
    {
        [Dependency] private readonly StatusEffectsSystem _status = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Update(float frameTime)
        {
            foreach (var comp in EntityQuery<VampireInfectionComponent>(true))
            {
                comp.TimeLeft = MathF.Max(0f, comp.TimeLeft - frameTime);

                if (comp.TimeLeft <= 45f)
                {
                    // Blink/blind effect (two‐arg overload)
                    _status.TryAddStatusEffect(comp.Owner, "TemporaryBlindness");

                    // Periodic thirst popup
                    if (comp.TimeLeft % 10f < frameTime)
                    {
                        _popup.PopupEntity(
                            "You feel a primal thirst growing…",
                            comp.Owner,
                            PopupType.SmallCaution);
                    }
                }

                if (comp.TimeLeft <= 0f)
                {
                    // Conversion complete: remove infection, add VampireComponent
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                }
            }
        }
    }
}
