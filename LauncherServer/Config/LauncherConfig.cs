
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace LauncherServer
{
    [aConfigAttributes("Configs/Launcher.xml")]
    public class LauncherConfig : aConfig
    {
        public int LauncherServerPort = 8000;
        public int Version = 1;
        public string Message = "Invalid launcher version : Please visit http://AllPrivateServer.com/";

        public RpcClientConfig RpcInfo = new RpcClientConfig("127.0.0.1", "127.0.0.1", 6800);
        public LogInfo LogLevel = new LogInfo();
    }
}