using Content.Shared.Popups;
using Content.Server._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;            // for HumanoidComponent
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

            // Start infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Flavor popup
            var name = EntityManager.GetComponent<MetaDataComponent>(target).EntityName;
            var popup = Loc.GetString("vamp-bite-popup", ("{$victim}", name));
            _popup.PopupEntity(popup, target, PopupType.LargeCaution);

            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}