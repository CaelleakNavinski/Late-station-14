using System;
using Content.Shared.StatusEffect;               // ← StatusEffectsSystem & TimeSpan API
using Content.Shared.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Content.Server.Faction;                    // ← FactionSystem

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireInfectionSystem : EntitySystem
    {
        [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly FactionSystem _faction = default!;

        public override void Update(float frameTime)
        {
            foreach (var comp in EntityQuery<VampireInfectionComponent>(true))
            {
                // Tick down
                comp.TimeLeft = Math.Max(0f, comp.TimeLeft - frameTime);

                // Warn & blur when under 45 seconds
                if (comp.TimeLeft <= 45f)
                {
                    _statusEffects.TryAddStatusEffect(
                        comp.Owner,
                        "TemporaryBlindness",
                        TimeSpan.FromSeconds(1f)
                    );

                    if (comp.TimeLeft % 10f < frameTime)
                    {
                        _popup.PopupEntity(
                            "You feel a primal thirst growing…",
                            comp.Owner,
                            PopupType.SmallCaution
                        );
                    }
                }

                // Conversion complete
                if (comp.TimeLeft <= 0f)
                {
                    EntityManager.RemoveComponent<VampireInfectionComponent>(comp.Owner);
                    EntityManager.AddComponent<VampireComponent>(comp.Owner);
                    _faction.SetFaction(comp.Owner, "VampireFaction");
                }
            }
        }
    }
}
