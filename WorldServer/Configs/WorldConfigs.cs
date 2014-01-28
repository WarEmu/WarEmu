
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace WorldServer
{
    [aConfigAttributes("Configs/World.xml")]
    public class WorldConfigs : aConfig
    {
        public RpcClientConfig AccountCacherInfo = new RpcClientConfig("127.0.0.1", "127.0.0.1", 6800);
        public LogInfo LogLevel = new LogInfo();

        public DatabaseInfo CharacterDatabase = new DatabaseInfo();
        public DatabaseInfo WorldDatabase = new DatabaseInfo();

        public byte RealmId = 1;
        public int GlobalLootRate = 1;
        public int CommonLootRate = 1;
        public int UncommonLootRate = 1;
        public int RareLootRate = 1;
        public int VeryRareLootRate = 1;
        public int ArtifactLootRate = 1;
        public int GoldRate = 1;
        public int XpRate = 1;
        public int RenownRate = 1;

        public string ZoneFolder = "zones/";
    }
}
