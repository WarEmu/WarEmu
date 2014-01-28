
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class TCPServer : TCPManager
    {
        public TCPServer()
            : base()
        {
            PacketOut.SizeLen       = sizeof(UInt16);
            PacketOut.OpcodeInLen   = false;
            PacketOut.SizeInLen     = false;
            PacketOut.OpcodeReverse = false;
            PacketOut.SizeReverse   = false;
            PacketOut.Struct        = PackStruct.SizeAndOpcode;
        }

        protected override BaseClient GetNewClient()
        {
            GameClient client = new GameClient(this);

            return client;
        }

        public GameClient GetClientByAccount(GameClient Me,int AccountId)
        {
            lock(Clients)
                for(int i=0;i<Clients.Length;++i)
                    if (Clients[i] != null && Clients[i] != Me)
                    {
                        GameClient Client = Clients[i] as GameClient;
                        if (Client.HasAccount() && Client._Account.AccountId == AccountId)
                            return Client;
                    }

            return null;
        }

        public Player GetPlayerByName(string Name)
        {
            lock(Clients)
                for(int i=0;i<Clients.Length;++i)
                    if (Clients[i] != null)
                    {
                        GameClient Client = (Clients[i] as GameClient);
                        if (Client.IsPlaying() && Client.HasPlayer() && Client.Plr.Name.ToLower() == Name.ToLower())
                            return Client.Plr;
                    }

            return null;
        }
    }
}
