using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace FreeSCPs
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        public Dictionary<RoomType, string> Rooms { get; set; } = new Dictionary<RoomType, string>()
        {
            { RoomType.Lcz914, "914" },
            { RoomType.HczArmory, "HCZ_ARMORY" },
            { RoomType.LczArmory, "LCZ_ARMORY" },
            { RoomType.Hcz106, "106_PRIMARY" }
        };

        [Description("Should 106 be able to \"escape\" rooms with this plugin")]
        public bool Enable106RoomEscape { get; set; }

        public int SecondsBeforeMessage { get; set; } = 60;
        public string MessageContent { get; set; } = "<i>The door will be open to free you in 1 minute.</i>";

        public int SecondsBeforeOpening { get; set; } = 120;
        public string OpeningMessage { get; set; } = "<i>The door has been opened.</i>";
    }
}
