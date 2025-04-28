using Content.Shared.Popups;                           // SharedPopupSystem lives here
using Content.Shared._LateStation.Vampires.Components; // VampireInfectionComponent, VampireComponent
using Content.Shared._LateStation.Vampires.Events;     // VampireBiteActionEvent
using Content.Shared.Humanoid;                         // HumanoidComponent
using Robust.Shared.GameStates;                        // [Dependency], EntitySystem
using Robust.Server.GameObjects;                       // EntityManager.Resolve…

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireBiteSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Initialize()
        {
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
                ev.PopupText.Replace("{Victim}", 
                    EntityManager.GetComponent<MetaDataComponent>(target).EntityName),
                target,
                PopupType.LargeCaution);
            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
