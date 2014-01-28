
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using FrameWork;

namespace LobbyServer
{
    public class Client : BaseClient
    {
        public string Username = "";
        public string Token = "";

        public Client(TCPManager srv)
            : base(srv)
        {
            RSACryptoServiceProvider Prov = new RSACryptoServiceProvider();
        }

        public override void OnConnect()
        {
            Log.Debug("Client", "Connexion " + GetIp);
        }

        public override void OnDisconnect()
        {
            Log.Debug("Client", "Deconnexion " + GetIp);
        }

        private ushort Opcode = 0;
        private int m_expectSize = 0;
        public bool m_expectData = false;

        protected override void OnReceive(byte[] Packet)
        {
            lock (this)
            {
                PacketIn packet = new PacketIn(Packet, 0, Packet.Length);
                long byteLeft = packet.Length;

                while (byteLeft > 0)
                {
                    if (!m_expectData)
                    {
                        long StartPos = packet.Position;
                        m_expectSize = packet.DecodeMythicSize();
                        long EndPos = packet.Position;

                        long Diff = EndPos - StartPos;
                        byteLeft -= Diff;
                        if (m_expectSize <= 0)
                        {
                            packet.Opcode = packet.GetUint8();
                            packet.Size = (ulong)m_expectSize;
                            _srvr.HandlePacket(this, packet);
                            return;
                        }

                        if (byteLeft <= 0)
                            return;

                        Opcode = packet.GetUint8();
                        byteLeft -= 1;

                        m_expectData = true;
                    }
                    else
                    {
                        m_expectData = false;
                        if (byteLeft >= m_expectSize)
                        {
                            long Pos = packet.Position;

                            packet.Opcode = Opcode;
                            packet.Size = (ulong)m_expectSize;

                            _srvr.HandlePacket(this, packet);

                            byteLeft -= m_expectSize;
                            packet.Position = Pos;
                            packet.Skip(m_expectSize);
                        }
                        else
                        {
                            Log.Error("OnReceive", "Data count incorrect :" + byteLeft + " != " + m_expectSize);
                        }
                    }
                }

                packet.Dispose();
            }
        }

        public void SendTCPCuted(PacketOut Out)
        {

            long PSize = Out.Length - Out.OpcodeLen - PacketOut.SizeLen; // Size = Size-len-opcode

            byte[] Packet = new byte[PSize];
            Out.Position = Out.OpcodeLen + PacketOut.SizeLen;
            Out.Read(Packet, 0, (int)(PSize));

            List<byte> Header = new List<byte>(5);
            int itemcount = 1;
            while (PSize > 0x7f)
            {
                Header.Add((byte)((byte)(PSize) | 0x80));
                PSize >>= 7;
                itemcount++;
                if (itemcount >= Header.Capacity + 10)
                    Header.Capacity += 10;
            }

            Header.Add((byte)(PSize));
            Header.Add((byte)(Out.Opcode));

            Log.Tcp("Header", Header.ToArray(), 0, Header.Count);
            Log.Tcp("Packet", Packet, 0, Packet.Length);

            Log.Dump("Header", Header.ToArray(), 0, Header.Count);
            Log.Dump("Packet", Packet, 0, Packet.Length);

            SendTCP(Header.ToArray());
            SendTCP(Packet);

            Out.Dispose();
        }

        public void SendSegments(PacketOut Out)
        {
            long PSize = Out.Length - Out.OpcodeLen - PacketOut.SizeLen; // Size = Size-len-opcode

            byte[] Packet = new byte[PSize];
            Out.Position = Out.OpcodeLen + PacketOut.SizeLen;
            //Out.Read(Packet, 0, (int)(PSize));

            List<byte> Header = new List<byte>(5);
            int itemcount = 1;
            while (PSize > 0x7f)
            {
                Header.Add((byte)((byte)(PSize) | 0x80));
                PSize >>= 7;
                itemcount++;
                if (itemcount >= Header.Capacity + 10)
                    Header.Capacity += 10;
            }

            Header.Add((byte)(PSize));
            Header.Add((byte)(Out.Opcode));

            Log.Tcp("Header", Header.ToArray(), 0, Header.Count);
            SendTCP(Header.ToArray());


            // ugly needs to fix
            byte[] buffer;
            long bytesleft = PSize;
            int start = 0;
            while (PSize > 1460)
            {
                if (bytesleft < 1460) break;

                 buffer = new byte[(start + 1460) - start];
                 Out.Read(buffer, start, (start + 1460));
                 SendTCP(buffer);
                start += 1461;
                bytesleft -= 1461;
            }

            if (bytesleft > 0)
            {
                buffer = new byte[(start + bytesleft) - start];
                Out.Read(buffer, start, (int)(start + bytesleft));
                SendTCP(buffer);
            }


        }
    }
}
