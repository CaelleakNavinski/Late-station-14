using Content.Shared.IdentityManagement;                       // Identity.Entity
using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;        // VampireImmuneComponent
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;                                // HumanoidAppearanceComponent
using Content.Shared.Zombies;                                 // ZombieComponent
using Robust.Server.GameObjects;                              // EntityManager
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireBiteSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireBiteActionEvent>(OnBite);
        }

        private void OnBite(VampireBiteActionEvent ev)
        {
            var user = ev.Performer;
            var target = ev.Target;

            // Only bite humanoids (not zombies or animals)
            if (!HasComp<HumanoidAppearanceComponent>(target) || HasComp<ZombieComponent>(target))
            {
                _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-misc-popup", ("victim", Identity.Entity(target))),
                    user,
                    PopupType.SmallCaution);
                return;
            }

            // VampireImmuneComponent blocks all bites
            if (HasComp<VampireImmuneComponent>(target))
            {
                _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-aura-popup", ("victim", Identity.Entity(target, EntityManager))),
                    user,
                    PopupType.SmallCaution);
                _popup.PopupEntity(
                    Loc.GetString("vamp-victim-immune-aura-popup", ("vamp", Identity.Entity(user, EntityManager))),
                    target,
                    PopupType.SmallCaution);
                return;
            }

            // Already a vampire blocks bites
            if (HasComp<SharedVampireComponent>(target))
            {
                _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-other-vamp-popup", ("victim", Identity.Entity(target, EntityManager))),
                    user,
                    PopupType.SmallCaution);
                return;
            }

            // Start infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Flavor popup
            var popup = Loc.GetString("vamp-bite-popup", ("{$victim}", Identity.Entity(target, EntityManager)));
            _popup.PopupEntity(popup, target, PopupType.LargeCaution);

            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
