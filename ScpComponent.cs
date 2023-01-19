using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using UnityEngine;
using PlayerRoles;
using Interactables.Interobjects.DoorUtils;
using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;
using System;
using MEC;

namespace FreeSCPs
{
    public class ScpComponent : MonoBehaviour
    {
        public CoroutineHandle BroadcastCoroutine;
        public Player Player { get; private set; }
        public RoomType LastRoom { get; private set; } = RoomType.Unknown;

        public const float TimerRate = 0.5f;
        private float _timer = TimerRate;

        public DateTime StuckSince;
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0)
                return;

            _timer = TimerRate;
            var type = Player.CurrentRoom.Type;

            if (!Plugin.Instance.Config.Rooms.TryGetValue(type, out var doorName))
                return; // ply not stuck

            if (StuckSince == default)
            {
                var door = Door.Get(doorName);
                if (door.IsOpen || door.RequiredPermissions.RequiredPermissions.HasFlag(KeycardPermissions.ScpOverride))
                    return; // ply not stuck

                // ply now stuck
                LastRoom = Player.CurrentRoom.Type;
                StuckSince = DateTime.Now;
                BroadcastCoroutine = Timing.CallDelayed(Plugin.Instance.Config.SecondsBeforeMessage, () => Player.Broadcast(5, Plugin.Instance.Config.MessageContent));
                return;
            }

            if (LastRoom != type) // ply escaped
            {
                StuckSince = default;
                Timing.KillCoroutines(BroadcastCoroutine);
                return;
            }

            if ((DateTime.Now - StuckSince).TotalSeconds > Plugin.Instance.Config.SecondsBeforeOpening) // unstuck player
            {
                var pos = GetEscapePosition(doorName);
                Player.Broadcast(5, Plugin.Instance.Config.OpeningMessage);
                Player.Position = pos;
                StuckSince = default;
            }
        }

        private void Awake()
        {
            Player = Player.Get(gameObject);
            Exiled.Events.Handlers.Player.Died += CheckForDeath;
            Exiled.Events.Handlers.Player.ChangingRole += CheckForChangeRole;
        }

        private void OnDisable()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= CheckForChangeRole;
            Exiled.Events.Handlers.Player.Died -= CheckForDeath;
            Timing.KillCoroutines(BroadcastCoroutine);
        }

        private void CheckForDeath(DiedEventArgs ev)
        {
            if (ev.Player == Player)
                DestroyImmediate(this);
        }

        private void CheckForChangeRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole != RoleTypeId.Spectator && ev.Player == Player && ev.NewRole != RoleTypeId.None)
                DestroyImmediate(this);
        }

        public static string[] DontReverse =
        {
            "SERVERS_BOTTOM",
            "HCZ_ARMORY",
            "079_FIRST",
            "HID_RIGHT",
            "173_GATE",
            "HID_LEFT",
            "GATE_A",
            "GATE_B",
            "LCZ_WC",
            "GR18",
            "914",
            "HID"
        };

        public static Transform GetDoor(string door)
            => DoorNametagExtension.NamedDoors.TryGetValue(door, out DoorNametagExtension nametag) ? nametag.transform : null;
        
        public static Vector3 GetEscapePosition(string door)
        {
            Transform transform = GetDoor(door);

            if (transform is null)
                return default;

            return transform.position + (Vector3.up * 1.5f) + (transform.forward * (DontReverse.Contains(door) ? 3f : -3f));
        }
    }
}