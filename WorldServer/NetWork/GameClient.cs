
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Common;
using FrameWork;

public enum eClientState
{
    NotConnected = 0x00,
    Connecting = 0x01,
    CharScreen = 0x02,
    WorldEnter = 0x03,
    Playing = 0x04,
    Linkdead = 0x05,
    Disconnected = 0x06,
} ;

namespace WorldServer
{
    public class GameClient : BaseClient
    {
        public Account _Account = null;
        public Player Plr = null;

        public GameClient(TCPManager srv)
            : base(srv)
        {

        }

        public override void OnConnect()
        {
            Log.Debug("GameClient", "Connexion " + GetIp);
            State = (int)eClientState.Connecting;
        }
        public override void OnDisconnect()
        {
            Log.Success("GameClient", "Deconnexion " + GetIp + ",Id="+Id);
            if (Plr != null)
                Plr._Client = null;
        }

        public bool IsPlaying()
        {
            return State == (int)eClientState.Playing;
        }
        public bool HasPlayer()
        {
            return Plr != null;
        }
        public bool HasAccount()
        {
            return _Account != null;
        }

        private ushort Opcode = 0;
        private long PacketSize = 0;
        public bool ReadingData = false;
        public UInt16 SequenceID,SessionID,Unk1;
        public byte Unk2;
        public PacketIn packet = null;

        protected override void OnReceive(byte[] bytes)
        {
            PacketIn _Packet = new PacketIn(bytes, 0, bytes.Length);
            packet = _Packet;

            lock (this)
            {
                long PacketLength = packet.Length;

                while (PacketLength > 0)
                {
                    // Lecture du Header
                    if (!ReadingData)
                    {
                        if (PacketLength < 2)
                        {
                            Log.Error("OnReceive", "Header invalide " + PacketLength);
                            break;
                        }

                        PacketSize = packet.GetUint16();
                        PacketLength -= 2;

                        if (PacketLength < PacketSize + 10)
                            break;

                        packet.Size = (ulong)PacketSize+10;
                        packet = DeCrypt(packet);

                        SequenceID = packet.GetUint16();
                        SessionID = packet.GetUint16();
                        Unk1 = packet.GetUint16();
                        Unk2 = packet.GetUint8();
                        Opcode = packet.GetUint8();
                        PacketLength -= 8;

                        if (PacketLength > PacketSize + 2)
                        {
                            Log.Debug("OnReceive", "Packet contain multiple opcodes " + PacketLength + ">" + (PacketSize + 2));
                        }
                        ReadingData = true;
                    }
                    else
                    {
                        ReadingData = false;

                        if (PacketLength >= PacketSize + 2)
                        {
                            byte[] BPack = new byte[PacketSize+2];
                            packet.Read(BPack, 0, (int)(PacketSize + 2));

                            PacketIn Packet = new PacketIn(BPack, 0, BPack.Length);
                            Packet.Opcode = Opcode;
                            Packet.Size = (ulong)PacketSize;

                            if (Plr != null && Plr.IsInWorld())
                                Plr.ReceivePacket(Packet);
                            else
                                Server.HandlePacket(this, Packet);

                            Log.Tcp("PacketSize", BPack, 0, BPack.Length);

                            PacketLength -= PacketSize + 2;
                        }
                        else
                        {
                            Log.Error("OnReceive", "La taille du packet est inférieur au total recu :" + PacketLength + "<" + (PacketSize + 2));
                            break;
                        }
                    }
                }
            }
        }
    }
}
