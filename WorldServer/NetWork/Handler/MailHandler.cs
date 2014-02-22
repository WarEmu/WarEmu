using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class MailHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_MAIL, "onMail")]
        static public void F_MAIL(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (!cclient.IsPlaying() || !cclient.Plr.IsInWorld())
                return;

            Player Plr = cclient.Plr;

            byte Type = packet.GetUint8();
            Log.Success("MailInterface", "Recieved " + Type);
            switch (Type)
            {
                case 0: // Mailbox closed
                    {

                    } break;
                case 1: // Mail sent
                    {
                        Plr.MlInterface.BuildMail(packet);
                    } break;
                case 2: // Open mail
                case 3: // Return mail
                case 4: // Delete mail
                case 5: // Mark as read/unread
                case 7: // Take Item
                case 8: // Take money
                    {
                        byte Page = packet.GetUint8();
                        UInt32 Guid = ByteOperations.ByteSwap.Swap(packet.GetUint32());

                        Character_mail Mail = Plr.MlInterface.GetMail(Guid);

                        switch (Type)
                        {
                            case 2:
                                Log.Success("MailInterface", "Sending mail");
                                Plr.MlInterface.SendMail(Mail);
                                if (!Mail.Opened)
                                {
                                    Mail.Opened = true;
                                    CharMgr.SaveMail(Mail);
                                    Plr.MlInterface.SendMailCounts();
                                    Plr.MlInterface.SendMailBox();
                                }
                                break;
                            case 3:
                                //TODO
                                Plr.MlInterface.SendResult(GameData.MailResult.TEXT_MAIL_RESULT11);
                                break;
                            case 4:
                                Plr.MlInterface.RemoveMail(Mail);
                                Plr.MlInterface.SendMailCounts();
                                Plr.MlInterface.SendMailBox();
                                break;
                            case 5:
                                packet.Skip(4);
                                Mail.Opened = (packet.GetUint8() == 1);
                                CharMgr.SaveMail(Mail);
                                Plr.MlInterface.SendMailCounts();
                                Plr.MlInterface.SendMailBox();
                                break;
                            case 7:
                                //Mail items are not yet implemented
                                Plr.MlInterface.SendResult(GameData.MailResult.TEXT_MAIL_RESULT16);
                                break;
                            case 8:
                                if (Mail.Money == 0)
                                    return;
                                Plr.AddMoney(Mail.Money);
                                Mail.Money = 0;
                                CharMgr.SaveMail(Mail);
                                Plr.MlInterface.SendMailUpdate(Mail);
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
