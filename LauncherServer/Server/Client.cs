
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace LauncherServer
{
    public class Client : BaseClient
    {
        public long LastInfoRequest = 0;

        public Client(TCPManager srv)
            : base(srv)
        {

        }

        public override void OnConnect()
        {
            Log.Debug("Client", "Connexion " + GetIp);
        }

        public override void OnDisconnect()
        {
            Log.Debug("Client", "Deconnexion " + GetIp);
        }

        protected override void OnReceive(byte[] Packet)
        {
            lock (this)
            {
                PacketIn pack = new PacketIn(Packet, 0, Packet.Length);
                pack.Size = pack.GetUint32();
                pack.Opcode = pack.GetUint8();

                if (!Enum.IsDefined(typeof(Opcodes), (byte)pack.Opcode))
                {
                    Log.Error("OnReceive", "Opcode invalide : " + pack.Opcode);
                    return;
                }

                Server.HandlePacket((BaseClient)this, pack);
            }
        }
    }
}
