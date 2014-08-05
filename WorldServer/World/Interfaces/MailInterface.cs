using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
using GameData;

namespace WorldServer
{
    public class MailInterface : BaseInterface
    {
        IList<Character_mail> _Mails = new List<Character_mail>();
        UInt32 nextSend = 0;
        uint MAIL_PRICE = 30;

        public void Load(IList<Character_mail> Mails)
        {
            if (Mails != null)
            {
                foreach (Character_mail Mail in Mails)
                {
                    _Mails.Add(Mail);
                }
            }

            base.Load();
        }

        public void BuildMail(PacketIn packet)
        {
            Player Plr = GetPlayer();
            if (Plr == null)
                return;

            if (nextSend >= TCPServer.GetTimeStamp())
            {
                SendResult(GameData.MailResult.TEXT_MAIL_RESULT6);
                return;
            }

            // Recipient read
            packet.Skip(1);
            byte NameSize = packet.GetUint8();
            string Name = packet.GetString(NameSize);

            Character Receiver = CharMgr.GetCharacter(Name);

            if (Receiver == null || Receiver.Realm != (byte)Plr.Realm)
            {
                SendResult(GameData.MailResult.TEXT_MAIL_RESULT7);
                return;
            }

            if (Receiver.Name == Plr.Name) // You cannot mail yourself
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_PLAYER_CANT_MAIL_YOURSELF);
                return;
            }

            // Subject
            byte SubjectSize = packet.GetUint8();
            packet.Skip(1);
            string Subject = packet.GetString(SubjectSize);

            // Message
            byte MessageSize = packet.GetUint8();
            packet.Skip(1);
            string Message = packet.GetString(MessageSize);

            // Money
            UInt32 money = ByteOperations.ByteSwap.Swap(packet.GetUint32());

            // COD?
            byte cr = packet.GetUint8();

            // Item
            byte itemcounts = packet.GetUint8();

            if (!Plr.RemoveMoney((cr == 0 ? money : 0) + MAIL_PRICE))
            {
                SendResult(MailResult.TEXT_MAIL_RESULT8);
                return;
            }

            // Make a Mail
            Character_mail CMail = new Character_mail();
            CMail.Guid = CharMgr.GenerateMailGUID();
            CMail.CharacterId = Receiver.CharacterId;
            CMail.CharacterIdSender = Plr._Info.CharacterId;
            CMail.SenderName = Plr._Info.Name;
            CMail.ReceiverName = Name;
            CMail.SendDate = (uint)TCPManager.GetTimeStamp();
            CMail.Title = Subject;
            CMail.Content = Message;
            CMail.Money = money;
            CMail.Cr = true;
            CMail.Opened = false;

            Log.Debug("Mail", "Itemcount: " + itemcounts + "");


            for (byte i = 0; i < itemcounts; ++i)
            {
                UInt16 itmslot = ByteOperations.ByteSwap.Swap(packet.GetUint16());
                packet.Skip(2);

                ByteOperations.ByteSwap.Swap(itmslot);

                Item itm = Plr.ItmInterface.GetItemInSlot(itmslot);
                if (itm != null)
                {
                    CMail.Items.Add(new KeyValuePair<uint, ushort>(itm.Info.Entry, itm.CharItem.Counts));
                    Plr.ItmInterface.DeleteItem(itmslot, itm.CharItem.Counts, true);
                    itm.Owner = null;
                }

            }

            SendResult(MailResult.TEXT_MAIL_RESULT4);
            CharMgr.Database.AddObject(CMail);

            //If player exists let them know they have mail.
            Player mailToPlayer = Player.GetPlayer(Name);
            if (mailToPlayer != null)
                mailToPlayer.MlInterface.AddMail(CMail);


            nextSend = (uint)TCPServer.GetTimeStamp() + 5;
        }

        public void BuildPreMail(PacketOut Out, Character_mail Mail)
        {
            if (Mail == null)
                return;

            Out.WriteUInt32(0);
            Out.WriteUInt32((UInt32)Mail.Guid);
            Out.WriteUInt16((UInt16)(Mail.Opened ? 1 : 0));
            Out.WriteByte((byte)(Mail.AuctionType == 0 ? 100 : 0)); // Icon ID

            Out.WriteUInt32(Mail.SendDate); // Time
            Out.WriteUInt32(Mail.SendDate); // Sent time

            Out.WriteUInt32((UInt32)Mail.CharacterIdSender); // Sender ID

            Out.WriteByte(0); // 1 = localized name

            Out.WriteByte(0);
            Out.WriteStringToZero(Mail.SenderName);

            Out.WriteByte(0);
            Out.WriteStringToZero(Mail.ReceiverName);

            Out.WriteByte(0);
            Out.WriteStringToZero(Mail.Title);
            Out.WriteUInt32(0);

            Out.WriteUInt32(Mail.Money);
            Out.WriteUInt16((ushort)Mail.Items.Count);
            if (Mail.Items.Count > 0)
                Out.WriteByte(0);
            if (Mail.Items.Count > 8)
                Out.WriteByte(0);

            foreach (KeyValuePair<uint, ushort> item in Mail.Items)
            {
                Out.WriteUInt32(WorldMgr.GetItem_Info(item.Key).ModelId);
            }
        }

        public void SendMailCounts()
        {
            if (GetPlayer() == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
            Out.WriteByte(0x9);
            Out.WriteByte((byte)GameData.MailboxType.MAILBOXTYPE_PLAYER);

            UInt16 counts = 0;

            foreach (Character_mail Mail in _Mails)
                if (!Mail.Opened && Mail.AuctionType == 0)
                    counts++;

            Out.WriteUInt16(counts);
            GetPlayer().SendPacket(Out);

            UInt16 auctionCounts = 0;

            if (auctionCounts > 0)
            {
                PacketOut Auction = new PacketOut((byte)Opcodes.F_MAIL);
                Auction.WriteByte(0x9);
                Auction.WriteByte((byte)GameData.MailboxType.MAILBOXTYPE_AUCTION);
                Auction.WriteUInt16(auctionCounts);
                GetPlayer().SendPacket(Auction);
            }
        }

        public void SendMailBox()
        {
            if (GetPlayer() == null)
                return;

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
                Out.WriteUInt16(0);
                Out.WriteByte(1);
                GetPlayer().SendPacket(Out);
            }


            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
                Out.WriteUInt32(0x0E000000);
                Out.WriteUInt32(0x001E0AD7);
                Out.WriteUInt16(0xA33C);
                GetPlayer().SendPacket(Out);
            }


            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
                Out.WriteByte(0x0A);
                Out.WriteByte((byte)GameData.MailboxType.MAILBOXTYPE_PLAYER);
                Out.WriteByte(0);
                Out.WriteByte((byte)_Mails.Where(info => info.AuctionType == 0).Count());
                foreach (Character_mail Mail in _Mails)
                    if (Mail.AuctionType == 0)
                        BuildPreMail(Out, Mail);
                Out.WriteUInt16((UInt16)_Mails.Count());

                GetPlayer().SendPacket(Out);
            }

        }

        public void AddMail(Character_mail Mail)
        {
            _Mails.Add(Mail);
            SendMailCounts();
        }

        public void RemoveMail(Character_mail Mail)
        {
            _Mails.Remove(Mail);
            CharMgr.Database.DeleteObject(Mail);
            SendResult(MailResult.TEXT_MAIL_RESULT12);
        }

        public void SendMailUpdate(Character_mail Mail)
        {
            if (Mail == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
            Out.WriteByte(0x0B);
            Out.WriteByte(0);
            BuildPreMail(Out, Mail);
            GetPlayer().SendPacket(Out);
        }

        public Character_mail GetMail(UInt32 guid)
        {
            Character_mail mail = null;

            foreach (Character_mail Mail in _Mails)
            {
                if (Mail.Guid == guid)
                {
                    mail = Mail;
                    break;
                }
            }

            return mail;
        }

        public void SendMail(Character_mail Mail)
        {
            if (Mail == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
            Out.WriteByte(0x0D);
            Out.WriteByte(0);
            BuildPreMail(Out, Mail);

            Out.WriteUInt16((ushort)(Mail.Content.Length + 1));
            Out.WriteStringBytes(Mail.Content);
            Out.WriteByte(0);
   
            Out.WriteByte((byte)Mail.Items.Count);
            foreach (KeyValuePair<uint, ushort> item in Mail.Items)
            {
                Item_Info Req = WorldMgr.GetItem_Info(item.Key);
                Item.BuildItem(ref Out, null, Req, 0, item.Value);
            }
            GetPlayer().SendPacket(Out);
        }

        public void SendResult(GameData.MailResult Result)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MAIL);
            Out.WriteByte(0x0F);
            Out.WriteUInt16((ushort)Result);
            GetPlayer().SendPacket(Out);
        }
    }
}
