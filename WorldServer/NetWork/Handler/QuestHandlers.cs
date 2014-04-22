
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class QuestHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_QUEST, (int)eClientState.WorldEnter, "onQuest")]
        static public void F_QUEST(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            UInt16 QuestID = packet.GetUint16();
            UInt16 State = packet.GetUint16();
            UInt16 Unk1 = packet.GetUint16();
            byte Unk2 = packet.GetUint8();
            byte Unk3 = packet.GetUint8();
            UInt16 Unk4 = packet.GetUint16();
            UInt16 CreatureOID = packet.GetUint16();

            Creature Crea = cclient.Plr.Region.GetObject(CreatureOID) as Creature;

            if (Crea == null)
                return;

            switch (State)
            {
                case 1: // Show Quest
                    {
                        if (Crea.QtsInterface.HasQuestStarter(QuestID))
                            Crea.QtsInterface.BuildQuest(QuestID, cclient.Plr);

                    } break;

                case 2: // Accept Quest
                    {
                        if (Crea.QtsInterface.HasQuestStarter(QuestID))
                        {
                            if (cclient.Plr.QtsInterface.AcceptQuest(QuestID))
                            {
                                if (!Crea.QtsInterface.CreatureHasStartQuest(cclient.Plr))
                                {
                                    Crea.SendRemove(cclient.Plr);
                                    Crea.SendMeTo(cclient.Plr);
                                }
                            }
                        }

                    }break;

                case 3: // Quest Done
                    {
                        if (Crea.QtsInterface.hasQuestFinisher(QuestID))
                        {
                            if (cclient.Plr.QtsInterface.DoneQuest(QuestID))
                            {
                                Crea.SendRemove(cclient.Plr);
                                Crea.SendMeTo(cclient.Plr);
                            }
                        }

                    } break;

                case 4: // Quest Done Info
                    {

                        if (Crea.QtsInterface.hasQuestFinisher(QuestID))
                            Crea.QtsInterface.SendQuestDoneInfo(cclient.Plr, QuestID);
                        else if (Crea.QtsInterface.HasQuestStarter(QuestID))
                        {
                            Crea.QtsInterface.SendQuestInProgressInfo(cclient.Plr, QuestID);
                        }

                    } break;

                case 5: // Select Quest Reward
                    {
                        if (Crea.QtsInterface.hasQuestFinisher(QuestID))
                            cclient.Plr.QtsInterface.SelectRewards(QuestID, Unk3);

                    } break;

            };
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_QUEST, (int)eClientState.WorldEnter, "onRequestQuest")]
        static public void F_REQUEST_QUEST(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            UInt16 QuestID = packet.GetUint16();
            byte State = packet.GetUint8();

            switch (State)
            {
                case 0: // Show Quest
                    {
                        cclient.Plr.QtsInterface.SendQuest(QuestID);
                    } break;

                case 1: // Decline Quest
                    {
                        cclient.Plr.QtsInterface.DeclineQuest(QuestID);
                    } break;

                case 2: // Send To Group
                    {

                    } break;
            };
        }
    }
}
