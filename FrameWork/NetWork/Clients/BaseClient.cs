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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace FrameWork
{
    public class BaseClient
    {
        // Appeler lorsque le client recoit des données
        private static readonly AsyncCallback ReceiveCallback = OnReceiveHandler;
        static public bool DisconnectOnNullByte = true;

        private long _Id;
        public long Id
        { 
            get { return _Id; }
            set { _Id = value; }
        }

        private int _State = 0;
        public int State
        {
            get { return _State; }
            set { _State = value; }
        }

        #region Buffer&Socket

        protected byte[] _pBuf = new byte[2048];
        protected int _pBufOffset = 0;
        protected Socket _socket = null;

        public byte[] ReceiveBuffer
        { get { return _pBuf; } }

        public int ReceiveBufferOffset
        {
            get { return _pBufOffset; }
            set { _pBufOffset = value; }
        }

        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        #endregion

        #region Crypto

        private Dictionary<ICryptHandler, CryptKey[]> m_crypts = new Dictionary<ICryptHandler, CryptKey[]>();

        public bool AddCrypt(string name, CryptKey CKey,CryptKey DKey)
        {
            ICryptHandler Handler = Server.GetCrypt(name);

            if (Handler == null)
                return false;

            if (CKey == null)
                CKey = Handler.GenerateKey(this);

            if (DKey == null)
                DKey = Handler.GenerateKey(this);

            Log.Debug("Crypt", "Add crypt : " + name);

            if (m_crypts.ContainsKey(Handler))
                m_crypts[Handler] = new CryptKey[] { CKey , DKey };
            else 
                m_crypts.Add(Handler, new CryptKey[] { CKey , DKey } );

            return true;
        }

        public PacketIn DeCrypt(PacketIn packet)
        {
            if (m_crypts.Count <= 0)
                return packet;

            ulong opcode = packet.Opcode;
            ulong size = packet.Size;
            long StartPos = packet.Position;
            foreach (KeyValuePair<ICryptHandler, CryptKey[]> Entry in m_crypts)
            {
                try
                {
                    Log.Debug("Decrypt", "Decrypt with " + Entry.Key + ",Size="+packet.Size);

                    byte[] Buf = new byte[size];

                    long Pos = packet.Position;
                    packet.Read(Buf, 0, (int)Buf.Length);
                    packet.Position = Pos;

                    PacketIn Pack = Entry.Key.Decrypt(Entry.Value[1], Buf);
                    packet.Write(Pack.ToArray(), 0, Pack.ToArray().Length);

                    packet.Opcode = opcode;
                    packet.Size = size;
                }
                catch (Exception e)
                {
                    Log.Error("BaseClient", "Decrypt Error : " + e.ToString());
                    continue;
                }
            }

            Log.Tcp("Decrypt", packet.ToArray(), 0, packet.ToArray().Length);
            packet.Position = StartPos;
            return packet;
        }

        public PacketOut Crypt(PacketOut packet)
        {
            if (m_crypts.Count <= 0)
                return packet;

            byte[] Packet = packet.ToArray();

            Log.Tcp("SendTCP", Packet, 0, (int)Packet.Length);

            int Hpos = 0;
            Hpos += PacketOut.SizeLen;
            if (PacketOut.OpcodeInLen)
                Hpos += packet.OpcodeLen;

            byte[] Header = new byte[Hpos];
            byte[] ToCrypt = new byte[(packet.Length-Hpos)];
            int i;

            for (i = 0; i < Hpos; ++i)
                Header[i] = Packet[i];

            for (i = Hpos; i < Packet.Length; ++i)
                ToCrypt[i-Hpos] = Packet[i];
            
            try
            {
                foreach (KeyValuePair<ICryptHandler, CryptKey[]> Entry in m_crypts)
                {
                    ToCrypt = Entry.Key.Crypt(Entry.Value[0], ToCrypt);
                }
            }
            catch (Exception e)
            {
                Log.Error("BaseClient", "Crypt Error : " + e.ToString());
                return packet;
            }

            PacketOut Out = new PacketOut((byte)0);
            Out.Opcode = packet.Opcode;
            Out.OpcodeLen = packet.OpcodeLen;
            Out.Position = 0;
            Out.SetLength(0);

            byte[] Total = new byte[Header.Length + ToCrypt.Length];

            for (i = 0; i < Total.Length; ++i)
            {
                if (i < Header.Length)
                    Total[i] = Header[i];
                else
                    Total[i] = ToCrypt[i - Header.Length];
            }

            Out.Write(Total, 0, Total.Length);

            return Out;
        }

        #endregion

        public string GetIp
        {
            get
            {
                Socket s = _socket;
                if (s != null && s.Connected && s.RemoteEndPoint != null)
                    return s.RemoteEndPoint.ToString();

                return "disconnected";
            }
        }

        protected TCPManager _srvr;
        public TCPManager Server
        {
            get { return _srvr; }
        }

        public BaseClient(TCPManager srvr)
        {
            srvr.GenerateId(this);

            _srvr = srvr;

            if (srvr != null)
                _pBuf = srvr.AcquirePacketBuffer();

            m_tcpSendBuffer = srvr.AcquirePacketBuffer();

            _pBufOffset = 0;
        }

        public virtual void OnConnect()
        {

        }
        protected virtual void OnReceive(byte[] Packet)
        {

        }
        public virtual void OnDisconnect()
        {

        }

        public void BeginReceive()
        {
            if (_socket != null && _socket.Connected)
            {
                int bufSize = _pBuf.Length;

                if (_pBufOffset >= bufSize) //Do we have space to receive?
                {
                    Log.Debug("Client", GetIp + " disconnection was due to a buffer overflow!");
                    Log.Debug("Client", "_pBufOffset=" + _pBufOffset + "; buf size=" + bufSize);
                    Log.Debug("Client", _pBuf.ToString());

                    _srvr.Disconnect(this);
                }
                else
                {
                    _socket.BeginReceive(_pBuf, _pBufOffset, bufSize - _pBufOffset, SocketFlags.None, ReceiveCallback, this);
                }
            }
        }

        private static void OnReceiveHandler(IAsyncResult ar)
        {
            if (ar == null)
                return;

            BaseClient baseClient = null;

            try
            {
                baseClient = (BaseClient)ar.AsyncState;
                int numBytes = baseClient.Socket.EndReceive(ar);

                if (numBytes > 0 || (numBytes <=0 && DisconnectOnNullByte == false))
                {
                    Log.Tcp(baseClient.GetIp, baseClient.ReceiveBuffer, 0, numBytes);

                    byte[] buffer = baseClient.ReceiveBuffer;
                    int bufferSize = baseClient.ReceiveBufferOffset + numBytes;

                    byte[] Packet = new byte[bufferSize];
                    Buffer.BlockCopy(buffer, 0, Packet, 0, bufferSize);
                    baseClient.ReceiveBufferOffset = 0;
                    baseClient.OnReceive(Packet);

                    baseClient.BeginReceive();
                }
               else
                {
                    Log.Debug("BaseClient","disconnection of client (" + baseClient.GetIp + "), received bytes=" + numBytes);

                    baseClient._srvr.Disconnect(baseClient);
                }
            }
            catch (ObjectDisposedException)
            {
                if (baseClient != null)
                    baseClient._srvr.Disconnect(baseClient);
            }
            catch (SocketException e)
            {
                if (baseClient != null)
                {
                    Log.Info("BaseClient",string.Format("{0}  {1}", baseClient.GetIp, e.Message));

                    baseClient._srvr.Disconnect(baseClient);
                }
            }
            catch (Exception e)
            {
                Log.Error("BaseClient",e.ToString());

                if (baseClient != null)
                    baseClient._srvr.Disconnect(baseClient);
            }
        }

        public void CloseConnections()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Send);
                }
                catch
                {
                }

                try
                {
                    _socket.Close();
                }
                catch
                {
                }
            }

            byte[] buff = _pBuf;
            if (buff != null)
            {
                _pBuf = null;
                _srvr.ReleasePacketBuffer(buff);
            }
        }

        public void Disconnect()
        {
            try
            {
                _srvr.Disconnect(this);
            }
            catch (Exception e)
            {
                    Log.Error("Baseclient", e.ToString());
            }
        }

		#region TCP

        // Buffer en train d'être envoyé
		protected byte[] m_tcpSendBuffer;

        // Liste des packets a sender
		protected readonly Queue<byte[]> m_tcpQueue = new Queue<byte[]>(256);

        // True si un send est en cours
		protected bool m_sendingTcp;

        // Envoi un packet
        public void SendPacket(PacketOut packet)
		{
			//Fix the packet size
			packet.WritePacketLength();
            packet = Crypt(packet);

			//Get the packet buffer
			byte[] buf = packet.ToArray(); //packet.WritePacketLength sets the Capacity

			//Send the buffer
			SendTCP(buf);

            packet.Dispose();
		}

		public void SendTCP(byte[] buf)
		{
			if (m_tcpSendBuffer == null)
				return;

			//Check if client is connected
			if (Socket.Connected)
			{
				try
				{
					lock (m_tcpQueue)
					{
						if (m_sendingTcp)
						{
							m_tcpQueue.Enqueue(buf);
							return;
						}
						
						m_sendingTcp = true;
					}

                    if (m_crypts.Count <= 0)
                        Log.Tcp("SendTCP", buf, 0, buf.Length);
                    else
                        Log.Tcp("Crypted", buf, 0, buf.Length);

					Buffer.BlockCopy(buf, 0, m_tcpSendBuffer, 0, buf.Length);

					int start = Environment.TickCount;

					Socket.BeginSend(m_tcpSendBuffer, 0, buf.Length, SocketFlags.None, m_asyncTcpCallback, this);

					int took = Environment.TickCount - start;
					if (took > 100)
						Log.Notice("BaseClient","SendTCP.BeginSend took "+ took);
				}
				catch (Exception e)
				{
					// assure that no exception is thrown into the upper layers and interrupt game loops!
                    Log.Error("BaseClient", "SendTCP : " + e.ToString());
	    			_srvr.Disconnect(this);
				}
			}
		}

		protected static readonly AsyncCallback m_asyncTcpCallback = AsyncTcpSendCallback;

		protected static void AsyncTcpSendCallback(IAsyncResult ar)
		{
			if (ar == null)
			{
				Log.Error("BaseClient","AsyncSendCallback: ar == null");
				return;
			}

            BaseClient client = (BaseClient)ar.AsyncState;

			try
			{
                Queue<byte[]> q = client.m_tcpQueue;

				int sent = client.Socket.EndSend(ar);

				int count = 0;
                byte[] data = client.m_tcpSendBuffer;

                if (data == null)
                {
                    Log.Error("TcpCallBack", "Data == null");
                    return;
                }

				lock (q)
				{
					if (q.Count > 0)
					{
						count = CombinePackets(data, q, data.Length, client);
					}
					if (count <= 0)
					{
                        client.m_sendingTcp = false;
						return;
					}
				}

				int start = Environment.TickCount;

                if (client.m_crypts.Count <= 0)
                    Log.Tcp("SendTCPAs", data, 0, count);
                else
                    Log.Tcp("CryptedAs", data, 0, count);

				client.Socket.BeginSend(data, 0, count, SocketFlags.None, m_asyncTcpCallback, client);

				int took = Environment.TickCount - start;

            }
			catch (ObjectDisposedException)
			{
                client._srvr.Disconnect(client);
			}
			catch (SocketException)
			{
                client._srvr.Disconnect(client);
			}
			catch (Exception)
			{
                client._srvr.Disconnect(client);
			}
		}

		private static int CombinePackets(byte[] buf, Queue<byte[]> q, int length, BaseClient client)
		{
			int i = 0;
			byte[] pak = q.Dequeue();
			Buffer.BlockCopy(pak, 0, buf, i, pak.Length);
			i += pak.Length;
			return i;
		}

		public void SendTCPRaw(PacketOut packet)
		{
			SendTCP((byte[]) packet.GetBuffer().Clone());
		}

        #endregion


        public static T ByteToType<T>(PacketIn packet)
        {
            BinaryReader reader = new BinaryReader(packet);
            byte[] bytes = reader.ReadBytes(System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)System.Runtime.InteropServices.Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

    }
}
