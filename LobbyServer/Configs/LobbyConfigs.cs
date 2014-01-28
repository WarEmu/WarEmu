
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace LobbyServer
{
    [aConfigAttributes("Configs/Lobby.xml")]
    public class LobbyConfigs : aConfig
    {
        public int ClientPort = 8040;
        public string ClientVersion = "1.3.5";

        public RpcClientConfig RpcInfo = new RpcClientConfig("127.0.0.1", "127.0.0.1", 6800);
        public LogInfo LogLevel = new LogInfo();
    }
}
