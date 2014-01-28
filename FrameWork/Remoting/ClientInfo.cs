
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    [Serializable]
    public class RpcClientInfo
    {
        public string Name;
        public int RpcID;

        public string Ip;
        public int Port;

        public RpcClientInfo(string Name, string Ip, int Port, int RpcID)
        {
            this.Name = Name;
            this.Ip = Ip;
            this.Port = Port;
            this.RpcID = RpcID;
        }

        public string Description()
        {
            return "[" + RpcID + "]\t| " + Name + "| " + Ip + ":" + Port;
        }

        public bool Connected = true;
    }
}
