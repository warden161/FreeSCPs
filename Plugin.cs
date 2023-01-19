using System;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using PlayerRoles;

namespace FreeSCPs
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "FreeSCPs";
        public override string Author { get; } = "warden161";
        public override Version Version { get; } = new Version(0, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);

        public static Plugin Instance { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            base.OnDisabled();
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.Role.Side == Side.Scp || (Config.Enable106RoomEscape && ev.Player.Role.Type == RoleTypeId.Scp106))
                ev.Player.GameObject.AddComponent<ScpComponent>();
        }
    }
}
