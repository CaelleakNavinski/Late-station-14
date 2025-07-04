using System;
using System.Linq;
using Content.Server.AlertLevel;
using Content.Server.Antag;
using Content.Server.Chat.Systems;
using Content.Server.Mind;                              // MindSystem
using Content.Server.Roles;                             // RoleSystem, VampireRoleComponent
using Content.Server._LateStation.Roles;
using Content.Server.Station.Systems;
using Content.Shared.Mind.Components;                   // MindComponent
using Content.Shared.Actions;
using Content.Shared._LateStation.Vampires.Components;  // SharedVampireComponent
using Content.Server._LateStation.Vampires.Components; // VampireComponent, VampireMatriarchComponent
using Robust.Shared.IoC;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Server.Player;
using Robust.Shared.Maths;                              // Color

namespace Content.Server._LateStation.Vampires.Systems
{
    public sealed class VampireRoleSystem : EntitySystem
    {
        [Dependency] private readonly AntagSelectionSystem _antag = default!;
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly IPlayerManager _players = default!;
        [Dependency] private readonly AlertLevelSystem _alerts = default!;
        [Dependency] private readonly StationSystem _stations = default!;
        [Dependency] private readonly ChatSystem _chat = default!;
        [Dependency] private readonly MindSystem _mind = default!;
        [Dependency] private readonly RoleSystem _role = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<VampireComponent, ComponentInit>(OnVampireInit);
            SubscribeLocalEvent<VampireComponent, ComponentShutdown>(OnVampireShutdown);
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentInit>(OnMatriarchInit);
            SubscribeLocalEvent<VampireMatriarchComponent, ComponentShutdown>(OnMatriarchShutdown);
        }

        private void OnVampireInit(EntityUid uid, VampireComponent comp, ComponentInit args)
        {
            var total = EntityQuery<VampireComponent>().Count();
            var cap = Math.Max(3, (int)Math.Ceiling(_players.PlayerCount * 0.4f));
            if (total >= cap)
                TriggerSilverAlert(uid);

            // Add the "Vampire" mind‐role
            if (_mind.TryGetMind(uid, out var mindId, out var mind) && mind.UserId is { })
            {
                _role.MindAddRole(mindId, "MindRoleVampire");

                // Send briefing sound and text
                if (_players.TryGetSessionById(mind.UserId, out var session))
                {
                    var shared = EntityManager.GetComponent<SharedVampireComponent>(uid);
                    _antag.SendBriefing(session,
                        Loc.GetString("vamp-role-greeting"),
                        Color.Red,
                        shared.VampStartSound);
                }
            }
        }

        private void OnVampireShutdown(EntityUid uid, VampireComponent comp, ComponentShutdown args)
        {
            // Remove the "Vampire" mind‐role
            if (_mind.TryGetMind(uid, out var mindId, out _))
            {
                _role.MindRemoveRole<VampireRoleComponent>(mindId);
            }
        }

        private void OnMatriarchInit(EntityUid uid, VampireMatriarchComponent comp, ComponentInit args)
        {
            // Add the "Vampire Matriarch" mind‐role
            if (_mind.TryGetMind(uid, out var mindId, out _))
            {
                _role.MindAddRole(mindId, "MindRoleVampireMatriarch");
            }
        }

        private void OnMatriarchShutdown(EntityUid uid, VampireMatriarchComponent comp, ComponentShutdown args)
        {
            // Remove the "Vampire Matriarch" mind‐role
            if (_mind.TryGetMind(uid, out var mindId, out _))
            {
                _role.MindRemoveRole<VampireRoleComponent>(mindId);
            }
        }

        private void TriggerSilverAlert(EntityUid uid)
        {
            var station = _stations.GetOwningStation(uid);
            if (station == null)
                return;

            _alerts.SetLevel(station.Value, "Silver", playSound: true, announce: true, force: true);

            const string msg =
                "Scans have detected a significant escalation in vampiric activity aboard the station. " +
                "Remain within your departments and report any suspicious behavior to Security or the Chaplain. " +
                "Avoid isolated areas and travel in groups when possible.";
            _chat.DispatchStationAnnouncement(station.Value, msg, playDefaultSound: false);
        }
    }
}
