
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels.Tcp;

namespace FrameWork
{
    [Serializable]
    public class RpcServerConfig
    {
        public RpcServerConfig()
        {
        }

        public RpcServerConfig(string RpcIp, int RpcPort, int RpcClientStartingPort)
        {
            this.RpcIp = RpcIp;
            this.RpcPort = RpcPort;
            this.RpcClientStartingPort = RpcClientStartingPort;
        }

        public string RpcIp;
        public int RpcPort;
        public int RpcClientStartingPort;
    }

    public class RpcServer
    {
        static public int PING_TIME = 200;
        public TcpServerChannel Channel;
        public ServerMgr Mgr;

        public bool IsRunning = true;
        public Thread Pinger;
        
        // 0 = Client Class
        // 1 = Server Class
        public List<Type>[] RegisteredTypes;

        public int StartingPort;
        public string LocalIp;
        public int LocalPort;
        public int AllowedID;

        public RpcServer(int StartingPort, int AllowedID)
        {
            this.StartingPort = StartingPort;
            this.AllowedID = AllowedID;

            Pinger = new Thread(new ThreadStart(Ping));
            Pinger.Start();
        }

        public bool Start(string Ip, int Port)
        {
            try
            {
                if (Channel != null)
                    return false;

                LocalIp = Ip;
                LocalPort = Port;

                Log.Debug("RpcServer", "Start on : " + Ip + ":" + Port);

                Channel = new TcpServerChannel("Server" + Port, Port);
                ChannelServices.RegisterChannel(Channel, false);
                RegisteredTypes = RpcObject.RegisterHandlers(true, AllowedID);

                ServerMgr.Server = this;
                Mgr = GetLocalObject<ServerMgr>();
                Mgr.StartingPort = StartingPort;

                Log.Success("RpcServer", "Listening on : " + Ip + ":" + Port);
            }
            catch (Exception e)
            {
                Log.Error("RpcServer", e.Message);
                Log.Notice("RpcServer", "Can not start RPC : " + Ip + ":" + Port);

                return false;
            }

            return true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Ping()
        {
            while (IsRunning)
            {
                int Start = Environment.TickCount;

                if (Mgr != null)
                {
                    List<RpcClientInfo> Disconnected = new List<RpcClientInfo>();

                    foreach (RpcClientInfo Info in Mgr.GetClients())
                    {
                        if (!Info.Connected)
                            continue;

                        try
                        {
                            GetObject<ClientMgr>(Info).Ping();
                        }
                        catch (Exception e)
                        {
                            Log.Error("RpcServer", e.ToString());
                            Log.Notice("RpcServer", Info.Description() + " | Disconnected");

                            Disconnected.Add(Info);
                            Mgr.Remove(Info.RpcID);
                        }
                    }

                    if (Disconnected.Count > 0)
                    {
                        foreach (RpcClientInfo ToDisconnect in Disconnected)
                        {
                            foreach (Type type in RegisteredTypes[0])
                                GetLocalObject(type).OnClientDisconnected(ToDisconnect);

                            foreach (RpcClientInfo Info in Mgr.GetClients())
                            {
                                foreach (Type type in RegisteredTypes[1])
                                    RpcServer.GetObject(type, Info.Ip, Info.Port).OnClientDisconnected(ToDisconnect);
                            }
                        }
                    }
                }

                int Diff = Environment.TickCount - Start;
                if (Diff < PING_TIME)
                    Thread.Sleep(PING_TIME - Diff);
            }
        }

        public T GetObject<T>(string Name) where T : RpcObject
        {
            RpcClientInfo Info = Mgr.GetClient(Name);
            if (Info == null)
            {
                Log.Error("RpcServer", "Can not find client : " + Name);
                return null;
            }

            return GetObject<T>(Info);
        }

        public T GetLocalObject<T>() where T : RpcObject
        {
            return RpcServer.GetObject(typeof(T),LocalIp,LocalPort) as T;
        }

        public RpcObject GetLocalObject(Type type)
        {
            return RpcServer.GetObject(type, LocalIp, LocalPort);
        }


        static public T GetObject<T>(RpcClientInfo Info) where T : RpcObject
        {
            return GetObject(typeof(T), Info.Ip, Info.Port) as T;
        }

        static public RpcObject GetObject(Type type, string Ip, int Port)
        {
            return Activator.GetObject(type, "tcp://" + Ip + ":" + Port + "/" + type.Name) as RpcObject;
        }
    }
}
