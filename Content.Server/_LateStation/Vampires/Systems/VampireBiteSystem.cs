using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;            // for HumanoidComponent
using Content.Shared.Zombies;
using Content.Shared.Actions;             // for ActionsComponent (if needed)
using Robust.Shared.GameStates;           // EntitySystem
using Robust.Server.GameObjects;          // EntityManager, SharedPopupSystem
using Robust.Shared.IoC;                  // [Dependency]
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

            if (!HasComp<HumanoidAppearanceComponent>(target) || HasComp<ZombieComponent>(target))
            {
                 _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-misc-popup", ("victim", Identity.Entity(target))),
                    user,
                    PopupType.SmallCaution);
                return;
            }
            
            // 1) VampireImmuneComponent blocks all bites
            if (HasComp<VampireImmuneComponent>(target))
            {
                _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-aura-popup", ("victim", Identity.Entity(target))),
                    user,
                    PopupType.SmallCaution);
                _popup.PopupEntity(
                    Loc.GetString("vamp-victim-immune-aura-popup", ("vamp", Identity.Entity(user))),
                    target,
                    PopupType.SmallCaution);
                return;
            }

            // 2) SharedVampireComponent on target also blocks bites
            if (HasComp<SharedVampireComponent>(target))
            {
                _popup.PopupEntity(
                    Loc.GetString("vamp-target-immune-other-vamp-popup", ("victim", Identity.Entity(target))),
                    user,
                    PopupType.SmallCaution);
                return;
            }
            // Start infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Flavor popup
            var popup = Loc.GetString("vamp-bite-popup", ("{$victim}", Identity.Entity(target)));
            _popup.PopupEntity(popup, target, PopupType.LargeCaution);

            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
