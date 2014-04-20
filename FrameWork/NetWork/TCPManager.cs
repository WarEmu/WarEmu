/*
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Reflection;


namespace FrameWork
{
    public class TCPManager
    {
        #region Manager

        static private Dictionary<string, TCPManager> _Tcps = new Dictionary<string, TCPManager>();
        static private TCPManager ConvertTcp<T>() where T : TCPManager, new()
        {
            if (!typeof(T).IsSubclassOf(typeof(TCPManager)))
                return null;

            T Tcp = new T();
            TCPManager TTcp = (TCPManager)Tcp;

            return TTcp;
        }
        static public bool Listen<T>(int port, string Name) where T : TCPManager, new()
        {
            try
            {
                if (Name.Length <= 0)
                    return false;

                if (port <= 0)
                    return false;

                if (_Tcps.ContainsKey(Name))
                    return false;

                TCPManager Tcp = ConvertTcp<T>();

                if (Tcp == null || !Tcp.Start(port))
                {
                    Log.Error(Name, "Can not start server on port : " + port);
                    return false;
                }

                _Tcps.Add(Name, Tcp);
            }
            catch (Exception e)
            {
                Log.Error("Listen", "Error : " + e.ToString());
                return false;
            }

            return true;
        }
        static public bool Connect<T>(string IP, int port, string Name) where T : TCPManager, new()
        {
            if (Name.Length <= 0)
                return false;

            if (port <= 0)
                return false;

            if (_Tcps.ContainsKey(Name))
                return false;

            TCPManager Tcp = ConvertTcp<T>();
            if (Tcp == null || Tcp.Start(IP, port))
            {
                Log.Error(Name, "Can not connect to : " + IP + ":" + port);
                return false;
            }

            _Tcps.Add(Name, Tcp);

            return true;
        }
        static public T GetTcp<T>(string Name)
        {
            if (_Tcps.ContainsKey(Name))
                return (T)Convert.ChangeType(_Tcps[Name], typeof(T));

            return (T)Convert.ChangeType(null, typeof(T));
        }

        #endregion

        private TcpListener Listener = null;
        private Socket Client = null;
        private readonly AsyncCallback _asyncAcceptCallback;

        private bool LoadCryptHandlers = true;
        private bool LoadPacketHandlers = true;

        public readonly PacketFunction[] m_packetHandlers = new PacketFunction[0xFFFFFF];
        public readonly Dictionary<string,ICryptHandler> m_cryptHandlers = new Dictionary<string,ICryptHandler>();

        #region Clients

        // Liste des clients connectés
        static public int MAX_CLIENT = 65000;

        private ReaderWriterLockSlim ClientRWLock = new ReaderWriterLockSlim();
        public BaseClient[] Clients = new BaseClient[MAX_CLIENT];
        public int GetClientCount()
        {
            int Count = 0;

            LockReadClients();

            for (int i = 0; i < Clients.Length; ++i)
                if (Clients[i] != null)
                    ++Count;

            UnLockReadClients();

            return Count;
        }

        public void GenerateId(BaseClient Client)
        {
            LockWriteClients();

            for (int i = 10; i < Clients.Length; ++i)
            {
                if (Clients[i] == null)
                {
                    Client.Id = i;
                    Clients[i] = Client;
                    break;
                }
            }

            UnLockWriteClients();
        }

        public void RemoveClient(BaseClient Client)
        {
            LockWriteClients();

            if (Clients[Client.Id] == Client)
                Clients[Client.Id] = null;

            UnLockWriteClients();
        }
        public void RemoveClient(int Id)
        {
            LockWriteClients();

                if (Id >= 0 && Id < Clients.Length)
                    Clients[Id] = null;

           UnLockWriteClients();
        }

        public BaseClient GetClient(int Id)
        {
            LockReadClients();

            if (Id >= 0 && Id < Clients.Length)
                return Clients[Id];

            UnLockReadClients();

            return null;
        }

        public void LockReadClients()
        {
            ClientRWLock.EnterReadLock();
        }
        public void UnLockReadClients()
        {
            ClientRWLock.ExitReadLock();
        }
        public void LockWriteClients()
        {
            ClientRWLock.EnterWriteLock();
        }
        public void UnLockWriteClients()
        {
            ClientRWLock.ExitWriteLock();
        }

        #endregion

        public TCPManager()
        {
            _asyncAcceptCallback = new AsyncCallback(ConnectingThread);
        }

        static public int GetTimeStamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        static public long GetTimeStampMS()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private bool InitSocket(int port)
        {
            try
            {
                Listener = new TcpListener(port);
                Listener.Server.ReceiveBufferSize = BUF_SIZE;
                Listener.Server.SendBufferSize = BUF_SIZE;
                Listener.Server.NoDelay = false;
                Listener.Server.Blocking = false;

                AllocatePacketBuffers();
            }
            catch (Exception e)
            {
                Log.Error("InitSocket", e.ToString());
                return false;
            }

            return true;
        }

        private bool InitSocket(string Ip,int port)
        {
            try
            {
                IPHostEntry LSHOST = Dns.GetHostEntry(Ip);

                IPEndPoint EndPoint = new IPEndPoint(LSHOST.AddressList[0], port);
                Client = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Client.Connect(EndPoint);

                if (Client.Connected)
                    ConnectingThread((IAsyncResult)Client);

                AllocatePacketBuffers();
            }
            catch (Exception e)
            {
                Log.Error("InitSocket", e.ToString());
                return false;
            }

            return true;
        }

        public virtual bool Start(string IpToConnect, int port)
        {
            Log.Info("TCPManager", "Starting...");

            if (!InitSocket(IpToConnect,port))
                return false;

            try
            {
                if(LoadPacketHandlers)
                    LoadPacketHandler();

                if(LoadCryptHandlers)
                    LoadCryptHandler();

                Client.Connect(IpToConnect, port);

                if (!Client.Connected)
                    return false;

                Log.Success("TCPManager", "Client connected at : " + IpToConnect+":"+port);
            }
            catch (SocketException e)
            {
                Log.Error("TCPManager", e.ToString());
                return false;
            }


            return true;
        }

        // Start Socket and threads
        public virtual bool Start(int port)
        {
            Log.Info("TCPManager", "Starting...");

            if (!InitSocket(port))
                return false;

            try
            {
                LoadPacketHandler();
                LoadCryptHandler();

                Listener.Start();
                Listener.BeginAcceptTcpClient(ConnectingThread, this);

                Log.Success("TCPManager", "Server listening to : " + Listener.LocalEndpoint.ToString());
            }
            catch (SocketException e)
            {
                Log.Error("TCPManager", e.ToString());
                return false;
            }


            return true;
        }

        // Stop all threads and incomming connections
        public virtual void Stop()
        {
            Log.Debug("TCPManager", "TCP Manager shutdown [" + Listener.LocalEndpoint.ToString() + "]");

            try
            {
                if (Listener != null)
                {
                    Listener.Stop();

                    Log.Debug("TCPManager", "Stop incoming connections");
                }
            }
            catch (Exception e)
            {
               Log.Error("TCPManager", e.ToString());
            }

            lock (Clients.SyncRoot)
            {
                for (int i = 0; i < Clients.Length; ++i)
                {
                    if (Clients[i] != null)
                    {
                        Clients[i].CloseConnections();
                        Clients[i] = null;
                    }
                }

            }
            
        }

        public void SendToAll(PacketOut Packet)
        {
            Packet.WritePacketLength();

            LockReadClients();

            for (int i = 0; i < Clients.Length; ++i)
            {
                if (Clients[i] != null)
                    Clients[i].SendTCP(Packet.ToArray());
            }

            UnLockReadClients();
        }

        #region Packet buffer pool

        // Taille maximal des packets
        public int BUF_SIZE = 65536;

        // Taille minimal du buffer pool
        public int POOL_SIZE = 1000;

        // Liste des packets
        private Queue<byte[]> m_packetBufPool;

        public int PacketPoolSize
        {
            get { return m_packetBufPool.Count; }
        }

        //  Allocation d'un nouveau packet
        private bool AllocatePacketBuffers()
        {
            m_packetBufPool = new Queue<byte[]>(POOL_SIZE);
            for (int i = 0; i < POOL_SIZE; i++)
            {
                m_packetBufPool.Enqueue(new byte[BUF_SIZE]);
            }

            Log.Debug("TCPManager", "Allocation of the buffer pool : " + POOL_SIZE.ToString());

            return true;
        }

        // Demande d'un nouveau packet
        public byte[] AcquirePacketBuffer()
        {
            lock (m_packetBufPool)
            {
                if (m_packetBufPool.Count > 0)
                    return m_packetBufPool.Dequeue();
            }

            Log.Notice("TCPManager", "The buffer pool is empty!");

            return new byte[BUF_SIZE];
        }

        // Relachement d'un packet
        public void ReleasePacketBuffer(byte[] buf)
        {
            if (buf == null)
                return;

            lock (m_packetBufPool)
            {
                m_packetBufPool.Enqueue(buf);
            }
        }

        protected virtual BaseClient GetNewClient()
        {
            return new BaseClient(this);
        }

        #endregion

        // Thread for incomming connection
        private void ConnectingThread(IAsyncResult ar)
        {
            Socket sock = null;

            try
            {
            
                if (Listener == null && Client == null)
                    return;

                if(Listener != null)
                    sock = Listener.EndAcceptSocket(ar);

                sock.SendBufferSize = BUF_SIZE;
                sock.ReceiveBufferSize = BUF_SIZE;
                sock.NoDelay = false;
                sock.Blocking = false;

                BaseClient baseClient = null;

                try
                {
                    string ip = sock.Connected ? sock.RemoteEndPoint.ToString() : "socket disconnected";
                    if(Listener != null)
                        Log.Info("TCPManager", "New Connection : " + ip);

                    if(Client != null)
                        Log.Info("TCPManager", "New connection to : " + ip);

                    baseClient = GetNewClient();
                    baseClient.Socket = sock;

                    baseClient.OnConnect();
                    baseClient.BeginReceive();
                }
                catch (SocketException)
                {
                    if (baseClient != null)
                        Disconnect(baseClient);
                }
                catch (Exception e)
                {
                    Log.Error("TCPManager", e.ToString());

                    if (baseClient != null)
                        Disconnect(baseClient);
                }
            }
            catch
            {
                if (sock != null) // Ne pas laisser le socket ouvert
                {
                    try
                    {
                        sock.Close();
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                if (Listener != null) // Quoi qu'il arrive on continu d'écouter le socket
                {
                    Listener.BeginAcceptSocket(_asyncAcceptCallback, this);
                }
            }
        }

        public virtual bool Disconnect(BaseClient baseClient)
        {

            RemoveClient(baseClient);

            try
            {
                baseClient.OnDisconnect();
                baseClient.CloseConnections();
            }
            catch (Exception e)
            {
                Log.Error("TCPManager", e.ToString());

                return false;
            }

            return true;
        }

        // PacketFunction
        public void LoadPacketHandler()
        {
            Log.Info("TCPManager", "Loading the Packet Handler");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    foreach (MethodInfo m in type.GetMethods())
                        foreach (object at in m.GetCustomAttributes(typeof(PacketHandlerAttribute), false))
                        {
                            PacketHandlerAttribute attr = at as PacketHandlerAttribute;
                            PacketFunction handler = (PacketFunction)Delegate.CreateDelegate(typeof(PacketFunction), m);

                            Log.Debug("TCPManager", "Registering handler for opcode : " + attr.Opcode.ToString("X8"));
                            m_packetHandlers[attr.Opcode] = handler;
                        }
                }
            }
        }

        //Charge les systemes de cryptographie
        public void LoadCryptHandler()
        {
            Log.Info("TCPManager", "Loading Crypt Handler");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    CryptAttribute[] crypthandler =
                        (CryptAttribute[])type.GetCustomAttributes(typeof(CryptAttribute), true);

                    if (crypthandler.Length > 0)
                    {
                        Log.Debug("TCPManager", "Registering crypt " + crypthandler[0]._CryptName);
                        m_cryptHandlers.Add(crypthandler[0]._CryptName, (ICryptHandler)Activator.CreateInstance(type));
                    }
                }
            }
        }

        public ICryptHandler GetCrypt(string name)
        {
            if (m_cryptHandlers.ContainsKey(name))
                return m_cryptHandlers[name];
            else 
                return null;
        }

        // Enregistre un handler
        public void RegisterPacketHandler(int packetCode, PacketFunction handler)
        {
            m_packetHandlers[packetCode] = handler;
        }

        public HashSet<ulong> Errors = new HashSet<ulong>();
        public void HandlePacket(BaseClient client, PacketIn Packet)
        {
            Log.Dump("packethandle", Packet.ToArray(), 0, Packet.ToArray().Length);
            if (client == null || Packet == null)
            {
                Log.Error("TCPManager", "Packet || Client == null");
                return;
            }

            PacketFunction packetHandler = null;

            if (Packet.Opcode < (ulong)m_packetHandlers.Length)
                packetHandler = m_packetHandlers[Packet.Opcode];
            else if (!Errors.Contains(Packet.Opcode))
            {
                Errors.Add(Packet.Opcode);
                Log.Error("TCPManager", "Can not handle :" + Packet.Opcode + "(" + Packet.Opcode.ToString("X8") + ")");
            }

            if (packetHandler != null)
            {
                PacketHandlerAttribute[] packethandlerattribs = (PacketHandlerAttribute[])packetHandler.GetType().GetCustomAttributes(typeof(PacketHandlerAttribute), true);
                if (packethandlerattribs.Length > 0)
                    if (packethandlerattribs[0].State > client.State)
                    {
                        Log.Error("TCPManager", "Can not handle packet ("+Packet.Opcode.ToString("X8")+"), Invalid client state ("+client.GetIp+")");
                        return;
                    }

                try
                {
                    packetHandler.Invoke(client,Packet);
                }
                catch (Exception e)
                {
                    Log.Error("TCPManager","Packet handler error :"+ Packet.Opcode + " " + e.ToString() );
                }
            }
            else if (!Errors.Contains(Packet.Opcode))
            {
                Errors.Add(Packet.Opcode);
                Log.Error("TCPManager", "Can not Handle opcode :" + Packet.Opcode + "(" + Packet.Opcode.ToString("X8") + ")");
            }
        }
    }
}
