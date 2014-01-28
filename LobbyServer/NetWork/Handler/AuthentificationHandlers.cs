
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
using Google.ProtocolBuffers;

namespace LobbyServer.NetWork.Handler
{

    public class AuthentificationHandlers : IPacketHandler
    {

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_VerifyProtocolReq, 0, "onVerifyProtocolReq")]
        static public void CMSG_VerifyProtocolReq(BaseClient client, PacketIn packet)
        {
            Client cclient = client as Client;

            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_VerifyProtocolReply);

            byte[] IV_HASH1 = { 0x01, 0x53, 0x21, 0x4d, 0x4a, 0x04, 0x27, 0xb7, 0xb4, 0x59, 0x0f, 0x3e, 0xa7, 0x9d, 0x29, 0xe9 };
            byte[] IV_HASH2 = { 0x49, 0x18, 0xa1, 0x2a, 0x64, 0xe1, 0xda, 0xbd, 0x84, 0xd9, 0xf4, 0x8a, 0x8b, 0x3c, 0x27, 0x20 };
            
            ByteString iv1 = ByteString.CopyFrom(IV_HASH1);
            ByteString iv2 = ByteString.CopyFrom(IV_HASH2);
            VerifyProtocolReply.Builder verify = VerifyProtocolReply.CreateBuilder();
            verify.SetResultCode(VerifyProtocolReply.Types.ResultCode.RES_SUCCESS);

            verify.SetIv1(ByteString.CopyFrom(IV_HASH1));
            verify.SetIv2(ByteString.CopyFrom(IV_HASH2));


            
            Out.Write(verify.Build().ToByteArray());
   

            cclient.SendTCPCuted(Out);

        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_AuthSessionTokenReq, 0, "onAuthSessionTokenReq")]
        static public void CMSG_AuthSessionTokenReq(BaseClient client, PacketIn packet)
        {
            Client cclient = client as Client;

            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_AuthSessionTokenReply);
        

            AuthSessionTokenReq.Builder authReq = AuthSessionTokenReq.CreateBuilder();
            authReq.MergeFrom(packet.ToArray());

            string session = Encoding.ASCII.GetString(authReq.SessionToken.ToByteArray());
           // Log.Debug("AuthSession", "session " + session);
            cclient.Username = "";                                  //username is not important anymore in 1.4.8
            cclient.Token = session;



            AuthSessionTokenReply.Builder authReply = AuthSessionTokenReply.CreateBuilder();
            authReply.SetResultCode(AuthSessionTokenReply.Types.ResultCode.RES_SUCCESS);

          
            Out.Write(authReply.Build().ToByteArray());

            cclient.SendTCPCuted(Out);
       
            
        /*   //TODO: need auth check

            if (Result != AuthResult.AUTH_SUCCESS)
                cclient.Disconnect();
            else
            {
                cclient.Username = Username;
                cclient.Token = Token;
            }*/
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_GetAcctPropListReq, 0, "onAcctPropListReq")]
        static public void CMSG_GetAcctPropListReq(BaseClient client, PacketIn packet)
        {
            Client cclient = client as Client;

            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetAcctPropListReply);
            byte[] val = { 0x08, 0x00 };
            Out.Write(val);
            cclient.SendTCPCuted(Out);

        }



        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_MetricEventNotify, 0, "onMetricEventNotify")]
        static public void CMSG_MetricEventNotify(BaseClient client, PacketIn packet)
        {
            //do nothing
        }


        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_GetClusterListReq, 0, "onGetServerListReq")]
        static public void CMSG_GetClusterListReq(BaseClient client, PacketIn packet)
        {
            Client cclient = client as Client;
            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetClusterListReply);
            byte[] ClustersList = Program.AcctMgr.BuildClusterList();

            Out.Write(ClustersList);
            cclient.SendTCPCuted(Out);

        }
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.CMSG_GetCharSummaryListReq, 1, "onGetCharacterSummaries")]
        static public void CMSG_GetCharSummaryListReq(BaseClient client, PacketIn packet)
        {
            Client cclient = client as Client;


            PacketOut Out = new PacketOut((byte)Opcodes.SMSG_GetCharSummaryListReply);

            Out.Write(new byte[2] { 0x08, 00 });
            cclient.SendTCPCuted(Out);
          
        }
    }
}
