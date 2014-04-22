
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class SocialInterface : BaseInterface
    {
        List<Character_social> _Socials = new List<Character_social>();

        public void Load(List<Character_social> Socials)
        {
            if(Socials != null)
                foreach (Character_social Social in Socials)
                    Load(Social);

            base.Load();
        }
        public EventInterface Load(Character_social Social)
        {
            if (Social == null || Social.DistCharacterId == 0)
                return null;

            EventInterface Interface = EventInterface.GetEventInterface((uint)Social.DistCharacterId);
            Interface.AddEventNotify(EventName.PLAYING, OnPlayerConnect, true);
            Interface.AddEventNotify(EventName.LEAVE, OnPlayerLeave, true);
            Social.Event = Interface;
            _Socials.Add(Social);

            return Interface;
        }

        public override void Save()
        {
            foreach (Character_social Social in _Socials)
                CharMgr.Database.SaveObject(Social);
        }

        public override void Update(long Tick)
        {

        }

        public override void Stop()
        {
            foreach(Character_social Social in _Socials)
                if(Social != null && Social.GetEvent<EventInterface>() != null)
                {
                    Social.GetEvent<EventInterface>().RemoveEventNotify(EventName.PLAYING, OnPlayerConnect);
                    Social.GetEvent<EventInterface>().RemoveEventNotify(EventName.LEAVE, OnPlayerLeave);
                }

            base.Stop();
        }

        private bool _Anon = false;
        private bool _Hide = false;
        public bool Anon
        {
            get { return _Anon; }
            set
            {
                _Anon = value;
                if(HasPlayer())
                    GetPlayer().SendLocalizeString("", _Anon ? GameData.Localized_text.TEXT_SN_ANON_ON : GameData.Localized_text.TEXT_SN_ANON_OFF);
            }
        }
        public bool Hide
        {
            get { return _Hide; }
            set
            {
                _Hide = value;
                if (HasPlayer())
                    GetPlayer().SendLocalizeString("", _Hide ? GameData.Localized_text.TEXT_SN_HIDE_ON : GameData.Localized_text.TEXT_SN_HIDE_OFF);
            }
        }

        #region Accessor

        public bool HasFriend(string Name)
        {
            Name = Name.ToLower();
            return _Socials.Find(info => info.Friend > 0 && info.DistName.ToLower() == Name) != null;
        }
        public bool HasIgnore(string Name)
        {
            Name = Name.ToLower();
            return _Socials.Find(info => info.Ignore > 0 && info.DistName.ToLower() == Name) != null;
        }
        public Character_social GetSocial(string Name)
        {
            Name = Name.ToLower();
            return _Socials.Find(info =>  info.DistName.ToLower() == Name);
        }

        #endregion

        #region Functions

        public void AddFriend(string Name)
        {
            Player Plr = GetPlayer();
            if (Plr == null)
                return;

            Name = Name.ToLower();

            if (Name.Length <= 0 || Name.ToLower() == Plr.Name.ToLower())
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_SN_FRIENDSLIST_ERR_ADD_SELF);
                return;
            }

            Character_social Social = GetSocial(Name);
            if (Social != null && Social.Friend > 0)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_SN_FRIENDSLIST_ERR_EXISTS);
                return;
            }

            Character Char = CharMgr.GetCharacter(Name);
            if (Char == null)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_SN_LISTS_ERR_PLAYER_NOT_FOUND);
                return;
            }

            EventInterface Event = null;

            if (Social != null)
            {
                Social.Friend = 1;
                CharMgr.Database.SaveObject(Social);

                Event = EventInterface.GetEventInterface((uint)Social.DistCharacterId);
            }
            else
            {
                Social = new Character_social();
                Social.CharacterId = Plr._Info.CharacterId;
                Social.DistName = Char.Name;
                Social.DistCharacterId = Char.CharacterId;
                Social.Friend = 1;
                Social.Ignore = 0;
                CharMgr.Database.AddObject(Social);

                Event = Load(Social);
            }

            SendFriends(Social);
            Plr.SendLocalizeString(Char.Name, GameData.Localized_text.TEXT_SN_FRIENDSLIST_ADD_SUCCESS);
        }
        public void RemoveFriend(string Name)
        {
            Log.Success("RemoveFriend", "Name=" + Name);
            Player Plr = GetPlayer();
            if (Plr == null)
                return;

            Name = Name.ToLower();
            Character_social Social = GetSocial(Name);
            if (Social == null)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_SN_LISTS_ERR_PLAYER_NOT_FOUND);
                return;
            }

            EventInterface Interface = Social.GetEvent<EventInterface>();
            if (Interface == null)
                return;

            Interface.RemoveEventNotify(EventName.PLAYING, OnPlayerConnect);
            Interface.RemoveEventNotify(EventName.LEAVE, OnPlayerLeave);

            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(0);
            Out.WriteByte(1);
            Out.WriteByte(1); // Count
            Out.WriteByte(1);
            Out.WriteUInt32((uint)Social.DistCharacterId);
            Plr.SendPacket(Out);
            Plr.SendLocalizeString(Social.DistName, GameData.Localized_text.TEXT_SN_FRIEND_REMOVE);

            _Socials.Remove(Social);
            CharMgr.Database.DeleteObject(Social);
        }

        #endregion

        #region Notify

        public bool OnPlayerConnect(Object Sender, object Args)
        {
            Log.Success("OnPlayerConnect", "Name=" + Sender.Name);

            Player Plr = GetPlayer();
            if (Plr == null)
                return false;

            Character_social Social = _Socials.Find(social => social != null && social.DistName.ToLower() == Sender.Name.ToLower());
            if(Social == null || Social.Friend <= 0)
                return true;


            Plr.SendLocalizeString(Sender.Name, GameData.Localized_text.TEXT_SN_FRIEND_LOGON);
            SendFriend(Sender.GetPlayer(),true);

            return false; // On ne le delete pas
        }
        public bool OnPlayerLeave(Object Sender, object Args)
        {
            Log.Success("OnPlayerLeave", "Name=" + Sender.Name);

            Player Plr = GetPlayer();
            if (Plr == null)
                return false;

            Character_social Social = _Socials.Find(social => social != null && social.DistName.ToLower() == Sender.Name.ToLower());
            if (Social == null || Social.Friend <= 0)
                return true;


            Plr.SendLocalizeString(Sender.Name, GameData.Localized_text.TEXT_SN_FRIEND_LOGOFF);
            SendFriend(Sender.GetPlayer(), false);

            return false;
        }

        #endregion

        #region Builder

        static public void BuildPlayerInfo(ref PacketOut Out,Character Char)
        {
            BuildPlayerInfo(ref Out, (uint)Char.CharacterId, Char.Name, false, 0, 0, 0);
        }
        static public void BuildPlayerInfo(ref PacketOut Out, Player Plr)
        {
            BuildPlayerInfo(ref Out, (UInt16)Plr._Value.CharacterId, Plr.Name, Plr.SocInterface.Anon ? false : true, Plr.Level, (UInt16)Plr._Info.Career, Plr._Value.ZoneId);
        }
        static public void BuildPlayerInfo(ref PacketOut Out,uint CharId, string Name, bool Online, byte Level, UInt16 Career, UInt16 Zone)
        {
            Out.WriteUInt32(CharId);
            Out.WriteByte(0);
            Out.WritePascalString(Name);
            Out.WriteByte(0);

            Out.WriteByte((byte)(Online ? 1 : 0));
            Out.WriteByte((byte)(Online ? 1 : 0));

            if (!Online)
                return;

            Out.WriteByte(Level);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Career);
            Out.WriteUInt16(Zone);
            Out.WriteUInt16(1); // Guild Size
            Out.WriteByte(0);
        }
        static public void BuildPlayerInfo(ref PacketOut Out, Character_social Social)
        {
            EventInterface Interface = Social.GetEvent<EventInterface>();
            if (Interface != null && Interface.HasPlayer())
                BuildPlayerInfo(ref Out, Interface.GetPlayer());
            else
                BuildPlayerInfo(ref Out, (uint)Social.DistCharacterId, Social.DistName, false, 0, 0, 0);
        }

        #endregion

        #region Packets

        public void SendFriend(Player Plr,bool online)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(0);
            Out.WriteByte(1);
            Out.WriteByte(1); // Count
            Out.WriteByte(0);
            BuildPlayerInfo(ref Out, (uint)Plr._Info.CharacterId, Plr._Info.Name, online, Plr._Value.Level, Plr._Info.Career, Plr._Value.ZoneId);
            GetPlayer().SendPacket(Out);
        }
        public void SendFriends(List<Character_social> Socials)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(0);
            Out.WriteByte(1);
            Out.WriteByte((byte)Socials.Count);
            Out.WriteByte(0);

            foreach (Character_social Social in Socials)
                BuildPlayerInfo(ref Out, Social);

            GetPlayer().SendPacket(Out);
        }
        public void SendFriends(Character_social Social)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(0);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.WriteByte(0);
            BuildPlayerInfo(ref Out, Social);
            GetPlayer().SendPacket(Out);
        }
        public void SendFriends()
        {
            SendFriends(_Socials.FindAll(social => social != null && social.Friend > 0));
        }


        public void SendPlayers(List<Player> Plrs)
        {
            if (!HasPlayer())
                return;

            Player Plr = GetPlayer();

            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(0);
            Out.WriteByte(4);
            Out.WriteByte((byte)Plrs.Count);
            foreach (Player Dist in Plrs)
            {
                SocialInterface.BuildPlayerInfo(ref Out, Dist);
            }
            Out.WriteByte(1);
            Plr.SendPacket(Out);
        }

        #endregion
    }
}
