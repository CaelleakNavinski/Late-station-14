using Content.Server.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;
using Robust.Shared.GameStates;
using Robust.Server.GameObjects;

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

            if (!EntityManager.HasComponent<HumanoidComponent>(target))
                return;
            if (EntityManager.HasComponent<VampireInfectionComponent>(target) ||
                EntityManager.HasComponent<VampireComponent>(target))
                return;

            EntityManager.AddComponent<VampireInfectionComponent>(target);
            _popup.PopupEntity(
                ev.PopupText.Replace("{Victim}", EntityManager.GetComponent<MetaDataComponent>(target).EntityName),
                target,
                PopupType.LargeCaution);
            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
