
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    [RpcAttribute(true, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 0)]
    public class ServerMgr : RpcObject
    {
        static public RpcServer Server;
        public int StartingPort;

        public delegate void Function();

        public List<RpcClientInfo> Clients = new List<RpcClientInfo>();

        public RpcClientInfo GetClient(string Name)
        {
            lock (Clients)
                return Clients.Find(info => info.Name == Name);
        }

        public RpcClientInfo GetClient(int RpcID)
        {
            lock (Clients)
                return Clients.Find(info => info.RpcID == RpcID);
        }

        public RpcClientInfo[] GetClients()
        {
            lock (Clients)
                return Clients.ToArray();
        }

        public void Remove(int RpcID)
        {
            lock (Clients)
                Clients.RemoveAll(info => info.RpcID == RpcID);
        }

        public RpcClientInfo Connect(string Name, string Ip)
        {
            RpcClientInfo Info = GetClient(Name);
            if (Info == null)
            {

                int RpcId = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

                Info = new RpcClientInfo(Name, Ip, ++StartingPort, RpcId);
                Info.Connected = false;

                lock (Clients)
                    Clients.Add(Info);
            }

            Log.Debug("ServerMgr", Info.Description() + " | Connecting");

            return Info;
        }

        public bool Connected(int RpcID)
        {
            RpcClientInfo Info = GetClient(RpcID);
            if (Info == null)
                return false;

            Log.Success("ServerMgr", Info.Description() + " | Connected");

            foreach (Type type in Server.RegisteredTypes[0])
                Server.GetLocalObject(type).OnClientConnected(Info);

            foreach (RpcClientInfo ConnectedClient in GetClients())
            {
                if (Info.RpcID == ConnectedClient.RpcID)
                    continue;

                try
                {

                    foreach (Type type in Server.RegisteredTypes[1])
                    {
                        Log.Debug("ServerMgr", Info.Name + " Send to : " + ConnectedClient.Name + ",T=" + type);
                        RpcServer.GetObject(type, ConnectedClient.Ip, ConnectedClient.Port).OnClientConnected(Info);
                        RpcServer.GetObject(type, Info.Ip, Info.Port).OnClientConnected(ConnectedClient);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("ServerMgr", e.ToString());
                    Log.Notice("ServerMgr", "Invalid : " + ConnectedClient.Description());
                }
            }


            Info.Connected = true;

            return true;
        }

        public void Ping()
        {

        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
