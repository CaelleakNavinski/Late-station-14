using Content.Shared.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;          // ← required for HumanoidComponent
using Robust.Shared.GameStates;
using Robust.Server.GameObjects;

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

            // Only humanoids can be bitten
            if (!EntityManager.HasComponent<HumanoidComponent>(target))
                return;

            // Already infected or already a vampire?
            if (EntityManager.HasComponent<VampireInfectionComponent>(target) ||
                EntityManager.HasComponent<VampireComponent>(target))
                return;

            // Begin infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Popup flavor text
            var name = EntityManager.GetComponent<MetaDataComponent>(target).EntityName;
            _popup.PopupEntity(
                ev.PopupText.Replace("{Victim}", name),
                target,
                PopupType.LargeCaution
            );

            // Disarm the bite-toggle
            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
