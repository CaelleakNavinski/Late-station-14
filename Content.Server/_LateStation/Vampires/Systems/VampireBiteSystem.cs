using Content.Shared.Popups;
using Content.Shared._LateStation.Vampires.Components;
using Content.Shared._LateStation.Vampires.Events;
using Content.Shared.Humanoid;    // brings in HumanoidComponent
using Robust.Shared.GameStates;              // EntitySystem
using Robust.Server.GameObjects;             // EntityManager
using Robust.Shared.IoC;                     // [Dependency]

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

            // Only humanoids
            if (!EntityManager.HasComponent<HumanoidComponent>(target))
                return;

            // Already infected or already vampire?
            if (EntityManager.HasComponent<VampireInfectionComponent>(target) ||
                EntityManager.HasComponent<VampireComponent>(target))
                return;

            // Start infection
            EntityManager.AddComponent<VampireInfectionComponent>(target);

            // Flavor popup
            var name = EntityManager.GetComponent<MetaDataComponent>(target).EntityName;
            _popup.PopupEntity(
                ev.PopupText.Replace("{Victim}", name),
                target,
                PopupType.LargeCaution);

            // Disarm the bite toggle on the user
            EntityManager.RemoveComponent<VampireBiteToggleComponent>(user);
        }
    }
}
