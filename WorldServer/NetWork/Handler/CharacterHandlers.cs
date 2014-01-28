
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class CharacterHandlers : IPacketHandler
    {
        public struct CreateInfo
        {
            public byte slot, race, career, sex, model;
            public UInt16 NameSize;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_CREATE_CHARACTER, "onCreateCharacter")]
        static public void F_CREATE_CHARACTER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            CreateInfo Info;

            Info.slot = packet.GetUint8();
            Info.race = packet.GetUint8();
            Info.career = packet.GetUint8();
            Info.sex = packet.GetUint8();
            Info.model = packet.GetUint8();
            Info.NameSize = packet.GetUint16();
            packet.Skip(2);

            byte[] Traits = new byte[8];
            packet.Read(Traits, 0, Traits.Length);
            packet.Skip(7);

            string Name = packet.GetString(Info.NameSize);

            if (!CharMgr.NameIsUsed(Name))
            {

                CharacterInfo CharInfo = CharMgr.GetCharacterInfo(Info.career);
                if (CharInfo == null)
                {
                    Log.Error("ON_CREATE", "Can not find career :" + Info.career);
                    return;
                }

                Log.Success("OnCreate", "Creating new Character : " + Name);

                Character Char = new Character();
                Char.AccountId = cclient._Account.AccountId;
                Char.bTraits = Traits;
                Char.Career = Info.career;
                Char.CareerLine = CharInfo.CareerLine;
                Char.ModelId = Info.model;
                Char.Name = Name;
                Char.Race = Info.race;
                Char.Realm = CharInfo.Realm;
                Char.RealmId = Program.Rm.RealmId;
                Char.Sex = Info.sex;

                if (!CharMgr.CreateChar(Char))
                {
                    Log.Error("CreateCharacter", "Hack : can not create more than 10 characters!");
                    return;
                }

                Character_items Citm = null;
                CharacterInfo_item[] Items = CharMgr.GetCharacterInfoItem(Char.CareerLine);

                for (int i = 0; i < Items.Length; ++i)
                {
                    if (Items[i] == null)
                        continue;

                    Citm = new Character_items();
                    Citm.Counts = Items[i].Count;
                    Citm.CharacterId = Char.CharacterId;
                    Citm.Entry = Items[i].Entry;
                    Citm.ModelId = Items[i].ModelId;
                    Citm.SlotId = Items[i].SlotId;
                    CharMgr.CreateItem(Citm);
                }

                Character_value CInfo = new Character_value();
                CInfo.CharacterId = Char.CharacterId;
                CInfo.Level = 1;
                CInfo.Money = 0;
                CInfo.Online = false;
                CInfo.RallyPoint = CharInfo.RallyPt;
                CInfo.RegionId = CharInfo.Region;
                CInfo.Renown = 0;
                CInfo.RenownRank = 1;
                CInfo.RestXp = 0;
                CInfo.Skills = CharInfo.Skills;
                CInfo.Speed = 100;
                CInfo.WorldO = CharInfo.WorldO;
                CInfo.WorldX = CharInfo.WorldX;
                CInfo.WorldY = CharInfo.WorldY;
                CInfo.WorldZ = CharInfo.WorldZ;
                CInfo.Xp = 0;
                CInfo.ZoneId = CharInfo.ZoneId;

                CharMgr.Database.AddObject(CInfo);

                Char.Value = new Character_value[1] { CInfo };
            }


            PacketOut Out = new PacketOut((byte)Opcodes.F_SEND_CHARACTER_RESPONSE);
            Out.WritePascalString(cclient._Account.Username);
            cclient.SendTCP(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DELETE_CHARACTER, "onDeleteCharacter")]
        static public void F_DELETE_CHARACTER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            byte Slot = packet.GetUint8();

            if (cclient._Account == null)
            {
                cclient.Disconnect();
                return;
            }

            CharMgr.RemoveCharacter(Slot, cclient._Account.AccountId);

            PacketOut Out = new PacketOut((byte)Opcodes.F_SEND_CHARACTER_RESPONSE);
            Out.WritePascalString(cclient._Account.Username);
            cclient.SendTCP(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DELETE_NAME, "onDeleteName")]
        static public void F_DELETE_NAME(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            string CharName = packet.GetString(30);
            string UserName = packet.GetString(20);

            byte Bad = 0;

            if (CharName.Length <= 0 || CharMgr.NameIsUsed(CharName))
                Bad = 1;

            Log.Debug("F_DELETE_NAME", "Bad=" + Bad + ",Name=" + CharName);

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHECK_NAME);
            Out.WriteString(CharName, 30);
            Out.WriteString(UserName, 20);
            Out.WriteByte(Bad);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(0);
            cclient.SendTCP(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DUMP_ARENAS_LARGE, "onDumpArenasLarge")]
        static public void F_DUMP_ARENAS_LARGE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (!cclient.HasAccount())
            {
                cclient.Disconnect();
                return;
            }

            byte CharacterSlot = packet.GetUint8();
            Character Char = CharMgr.GetAccountChar(cclient._Account.AccountId).GetCharacterBySlot(CharacterSlot);

            if (Char == null)
            {
                Log.Error("F_DUMP_ARENAS_LARGE", "Can not find character on slot : " + CharacterSlot);
                cclient.Disconnect();
                return;
            }

            if (cclient.Plr == null)
                cclient.Plr = Player.CreatePlayer(cclient, Char);

            PacketOut Out = new PacketOut((byte)Opcodes.F_WORLD_ENTER);
            Out.WriteUInt16(0x0608); // TODO
            Out.Fill(0, 20);
            Out.WriteString("38699", 5);
            Out.WriteString("38700", 5);
            Out.WriteString("0.0.0.0", 20);
            cclient.SendTCP(Out);
        }

        struct RandomNameInfo
        {
            public byte Race, Unk, Slot;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_RANDOM_NAME_LIST_INFO, "onRandomNameListInfo")]
        static public void F_RANDOM_NAME_LIST_INFO(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            RandomNameInfo Info = BaseClient.ByteToType<RandomNameInfo>(packet);

            Random_name[] Names = CharMgr.GetRandomNames();

            PacketOut Out = new PacketOut((byte)Opcodes.F_RANDOM_NAME_LIST_INFO);
            Out.WriteByte(0);
            Out.WriteByte(Info.Unk);
            Out.WriteByte(Info.Slot);
            Out.WriteUInt16(0);
            Out.WriteByte((byte)Names.Length);

            for (int i = Names.Length - 1; i >= 0; --i)
                Out.FillString(Names[i].Name, Names[i].Name.Length + 1);

            cclient.SendTCP(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_CHAR, "onRequestChar")]
        static public void F_REQUEST_CHAR(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            cclient.State = (int)eClientState.CharScreen;

            UInt16 Operation = packet.GetUint16();

            if (Operation == 0x2D58)
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_REQUEST_CHAR_ERROR);
                Out.WriteByte((byte)CharMgr.GetAccountRealm(cclient._Account.AccountId));
                cclient.SendTCP(Out);
            }
           
            else{
               
                 PacketOut Out = new PacketOut((byte)Opcodes.F_REQUEST_CHAR_RESPONSE);


                 Out.FillString(cclient._Account.Username, 25); // account name
                 Out.WriteByte(0xFF);
                 Out.WriteByte(CharMgr.MAX_SLOT); // Max characters 20
                 Out.WriteUInt16(0); // unk
                 Out.WriteByte(0); // name changing tokens, 1 will enable button
                 Out.WriteUInt16(0); //unk

                 byte[] Chars = CharMgr.BuildCharactersList(cclient._Account.AccountId);
                 Out.Write(Chars, 0, Chars.Length);
                 cclient.SendTCP(Out);
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_CHAR_TEMPLATES, "onRequestCharTemplates")]
        static public void F_REQUEST_CHAR_TEMPLATES(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            PacketOut Out = new PacketOut((byte)Opcodes.F_REQUEST_CHAR_TEMPLATES);
            Out.Write(new byte[0x11], 0, 0x11);
            cclient.SendTCP(Out);
        }
    }
}
