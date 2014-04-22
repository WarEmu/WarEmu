
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class AuthentificationHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_CONNECT, 0, "onConnect")]
        static public void F_CONNECT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            packet.Skip(8);
            UInt32 Tag = packet.GetUint32();
            string Token = packet.GetString(80);
            packet.Skip(21);
            string Username = packet.GetString(23);

            // TODO
            AuthResult Result = Program.AcctMgr.CheckToken(Username, Token);
            if (Result != AuthResult.AUTH_SUCCESS)
            {
                Log.Error("F_CONNECT", "Invalid Token =" + Username);
                cclient.Disconnect();
            }
            else
            {
                cclient._Account = Program.AcctMgr.GetAccount(Username);
                if (cclient._Account == null)
                {
                    Log.Error("F_CONNECT", "Invalid Account =" + Username);
                    cclient.Disconnect();
                }
                else
                {
                    //Log.Success("F_CONNECT", "MeId=" + cclient.Id);

                    GameClient Other = (cclient.Server as TCPServer).GetClientByAccount(cclient, cclient._Account.AccountId);
                    if (Other != null)
                        Other.Disconnect();

                    {
                        PacketOut Out = new PacketOut((byte)Opcodes.S_CONNECTED);
                        Out.WriteUInt32(0);
                        Out.WriteUInt32(Tag);
                        Out.WriteByte(Program.Rm.RealmId);
                        Out.WriteUInt32(1);
                        Out.WritePascalString(Username);
                        Out.WritePascalString(Program.Rm.Name);
                        Out.WriteByte(0);
                        Out.WriteUInt16(0);
                        cclient.SendPacket(Out);
                    }
                }
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PING, "onPing")]
        static public void F_PING(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            uint Timestamp = packet.GetUint32();

            PacketOut Out = new PacketOut((byte)Opcodes.S_PONG);
            Out.WriteUInt32(Timestamp);
            Out.WriteUInt64((UInt64)TCPManager.GetTimeStamp());
            Out.WriteUInt32((UInt32)(cclient.SequenceID+1));
            Out.WriteUInt32(0);
            cclient.SendPacket(Out);
        }

        public struct sEncrypt
        {
            public byte cipher, application, major, minor, revision, unk1;
        };

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_ENCRYPTKEY, "onEncryptKey")]
        static public void F_ENCRYPTKEY(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            sEncrypt Result = BaseClient.ByteToType<sEncrypt>(packet);

            string Version = Result.major + "." + Result.minor + "." + Result.revision;

            Log.Debug("F_ENCRYPTKEY", "Version = " + Version);

            if (Result.cipher == 0)
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_RECEIVE_ENCRYPTKEY);
                Out.WriteByte(1);
                cclient.SendPacket(Out);
            }
            else if (Result.cipher == 1)
            {
                byte[] EncryptKey = new byte[256];
                packet.Read(EncryptKey, 0, EncryptKey.Length);
                cclient.AddCrypt("RC4Crypto", new CryptKey(EncryptKey), new CryptKey(EncryptKey));
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DISCONNECT, 0,"onDisconnect")]
        static public void F_DISCONNECT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient._Account == null)
                return;

            GameClient OtherClient = (client.Server as TCPServer).GetClientByAccount(cclient, cclient._Account.AccountId);
            if (OtherClient != null)
                OtherClient.Disconnect();
        }
    }
}
