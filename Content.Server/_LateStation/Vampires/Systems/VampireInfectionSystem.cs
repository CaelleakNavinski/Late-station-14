using System;
using Content.Server.StatusEffects;
using Content.Shared.StatusEffects;
using Content.Shared.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.GameObjects;
using Content.Server.Faction;

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireInfectionSystem : EntitySystem
    {
        [Dependency] private readonly StatusEffectsSystem _status = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly FactionSystem _faction = default!;

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
                    _faction.SetFaction(comp.Owner, "VampireFaction");
                }
            }
        }
    }
}
