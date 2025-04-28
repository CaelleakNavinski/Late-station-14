using System;
using Content.Shared.StatusEffect;                     // StatusEffectsSystem
using Content.Shared.Popups;                           // SharedPopupSystem (if you need popups here too)
using Content.Shared._LateStation.Vampires.Components; // VampireInfectionComponent
using Robust.Shared.GameStates;                        // [Dependency], EntitySystem
using Robust.Shared.Timing;                            // frameTime
using Robust.Shared.GameObjects;                       // EntityQueryEnumerator

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
                comp.TimeLeft = Math.Max(0f, comp.TimeLeft - frameTime);

                if (comp.TimeLeft <= 45f)
                {
                    _status.TryAddStatusEffect(comp.Owner, "TemporaryBlindness", 1f);
                    if (comp.TimeLeft % 10f < frameTime)
                        _popup.PopupEntity(
                            "You feel a primal thirst growing…",
                            comp.Owner,
                            PopupType.SmallCaution);
                }

                if (comp.TimeLeft <= 0f)
                {
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                }
            }
        }
    }
}
