
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Channels.Tcp;
using System.Timers;
using System.Diagnostics;

namespace FrameWork
{
    [Serializable]
    public class RpcClientConfig
    {
        public RpcClientConfig()
        {

        }

        public RpcClientConfig(string RpcLocalIp, string RpcServerIp, int RpcServerPort)
        {
            this.RpcLocalIp = RpcLocalIp;
            this.RpcServerIp = RpcServerIp;
            this.RpcServerPort = RpcServerPort;
        }

        public string RpcLocalIp;
        public string RpcServerIp;
        public int RpcServerPort;
    }

    public class RpcClient
    {
        static public int PING_TIME = 200;
        public TcpClientChannel Channel;
        public TcpServerChannel ServerChannel;
        public ServerMgr Mgr;

        public bool IsRunning = true;
        public Thread Pinger;

        public RpcClientInfo Info;
        public List<Type>[] RegisteredTypes;

        public string ServerName;
        public string ServerIp;
        public bool Connecting;

        public string RpcServerIp;
        public int RpcServerPort;
        public int AllowedID;

        public RpcClient(string Name, string Ip, int AllowedID)
        {
            this.ServerName = Name;
            this.ServerIp = Ip;
            this.AllowedID = AllowedID;
            this.Connecting = false;

            Pinger = new Thread(new ThreadStart(CheckPing));
        }

        public bool Start(string Ip, int Port)
        {
            return Connect(Ip, Port);
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public bool Connect()
        {
            try
            {
                Connecting = true;

                if (Channel != null)
                    ChannelServices.UnregisterChannel(Channel);

                if (ServerChannel != null)
                    ChannelServices.UnregisterChannel(ServerChannel);

                Channel = null;
                ServerChannel = null;
                Mgr = null;
            }
            catch
            {

            }

            return Connect(RpcServerIp, RpcServerPort);
        }
        public bool Connect(string Ip, int Port)
        {
            Log.Debug("RpcClient", "Connect : " + Ip + ":" + Port);

            RpcServerIp = Ip;
            RpcServerPort = Port;
            Connecting = true;

            try
            {
                if (Channel != null)
                    return false;

                Log.Debug("RpcClient", "Connecting to : " + Ip);

                if(!Pinger.IsAlive)
                    Pinger.Start();

                Channel = new TcpClientChannel(ServerName, null);
                ChannelServices.RegisterChannel(Channel, false);

                Mgr = Activator.GetObject(typeof(ServerMgr), "tcp://" + Ip + ":" + Port + "/" + typeof(ServerMgr).Name) as ServerMgr;
                Info = Mgr.Connect(ServerName, ServerIp);

                if (Info == null)
                    return false;

                Log.Debug("RpcClient", "Listening on : " + ServerIp + ":" + Info.Port);

                ServerChannel = new TcpServerChannel("Server" + Info.RpcID, Info.Port);
                ChannelServices.RegisterChannel(ServerChannel, false);

                RegisteredTypes = RpcObject.RegisterHandlers(false, AllowedID);
                    foreach (Type t in RegisteredTypes[1])
                        RpcServer.GetObject(t, Info.Ip, Info.Port).MyInfo = Info;

                Mgr.Connected(Info.RpcID);

                Log.Success("RpcClient", "Connected to : " + ServerIp + ":" + RpcServerPort + ", Listen on : " + Info.Ip + ":" + Info.Port);

                foreach (Type t in RegisteredTypes[1])
                    RpcServer.GetObject(t, Info.Ip, Info.Port).OnServerConnected();

                Connecting = false;
            }
            catch (Exception e)
            {
                Log.Error("RpcClient", e.ToString());
                Log.Notice("RpcClient", "Can not start RPC : " + Ip + ":" + Port);

                Connecting = false;
                Mgr = null;
                Info = null;

                return false;
            }

            return true;
        }

        public void CheckPing()
        {
            while (IsRunning)
            {
                int Start = Environment.TickCount;

                if (!Connecting)
                {
                    try
                    {
                        if (Mgr != null)
                            Mgr.Ping();
                        else
                            Connect();
                    }
                    catch
                    {
                        foreach (Type t in RegisteredTypes[1])
                            RpcServer.GetObject(t, Info.Ip, Info.Port).OnServerDisconnected();

                        Connect();
                    }

                }
                int Diff = Environment.TickCount - Start;
                if (Diff < PING_TIME)
                    Thread.Sleep(PING_TIME - Diff);
            }
        }

        public T GetServerObject<T>() where T : RpcObject
        {
            try
            {
                T data = Activator.GetObject(typeof(T), "tcp://" + RpcServerIp + ":" + RpcServerPort + "/" + typeof(T).Name) as T;
                return data;
            }
            catch
            {
                return null;
            }
        }

        public T GetLocalObject<T>() where T : RpcObject
        {
            try
            {
                T data = Activator.GetObject(typeof(T), "tcp://" + Info.Ip + ":" + Info.Port + "/" + typeof(T).Name) as T;
                return data;
            }
            catch
            {
                return null;
            }
        }

        public T GetClientObject<T>(string Name) where T : RpcObject
        {
            RpcClientInfo Info = GetServerObject<ServerMgr>().GetClient(Name);
            if (Info == null)
                return GetLocalObject<T>();
            else
                return RpcServer.GetObject<T>(Info);
        }

        public T GetClientObject<T>(int RpcID) where T : RpcObject
        {
            RpcClientInfo Info = GetServerObject<ServerMgr>().GetClient(RpcID);
            if (Info == null)
                return GetLocalObject<T>();
            else
                return RpcServer.GetObject<T>(Info);
        }
    }
}
