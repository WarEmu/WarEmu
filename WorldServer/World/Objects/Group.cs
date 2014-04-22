using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class GroupInvitation
    {
        Player Owner;
        Player Invited;
        public Int32 Expire;

        public GroupInvitation(Player Own, Player Inv)
        {
            Log.Debug("GroupInvitation", "Created");
            Owner = Own;
            Invited = Inv;
            SendInvitation();
            Expire = TCPManager.GetTimeStamp() + 65;
        }

        void SendInvitation()
        {
            Log.Debug("GroupInvitation", "Sending invitation");
            if (Owner == null || Invited == null)
                return;

            Owner.Invitation = this;
            Invited.Invitation = this;

            Owner.SendLocalizeString(Invited.Name, GameData.Localized_text.TEXT_GROUP_INVITE_BEGIN);
            Invited.SendLocalizeString(Owner.Name, GameData.Localized_text.TEXT_GROUP_YOU_WERE_INVITED);
            Invited.SendDialog(0x0002, Owner.Name);
        }

        public void AcceptInvitation()
        {
            Log.Debug("GroupInvitation", "Accept invitation");

            if(Owner == null || Invited == null)
                return;

            Group group;

            if(Owner.GetGroup() != null)
                group = Owner.GetGroup();
            else
            {
                group = new Group();
                group.SetLeader(Owner);
            }

            if(group.IsFull())
	        {
                Log.Debug("GroupInvitation", "Group is full");
		        Invited.SendLocalizeString("", GameData.Localized_text.TEXT_PARTY_IS_FULL);
                Destroy();
		        return;
	        }

            Invited.SetGroup(group);
	        Owner.SetGroup(group);
            Destroy();
        }

        public void DeclineInvitation()
        {
            Log.Debug("GroupInvitation", "Decline invitation");

	        if(Owner != null && Invited != null)
                Owner.SendLocalizeString(Invited.Name, GameData.Localized_text.TEXT_ERROR_INVITE_DECLINED);

            Destroy();
        }

        public void Destroy()
        {
	        if(Owner != null)
                Owner.Invitation = null;
	        if(Invited != null)
                Invited.Invitation = null;
        }
    }

    public class Group
    {
        Player Leader;
        public List<Player> Members = new List<Player>();

        public void Update()
        {
            Log.Debug("Group", "Updating Group");

            if (Leader == null || Members.Count < 1 || (Members.Count == 1 && Leader == Members.First()))
            {
                Delete();
                return;
            }

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteUInt16(0x0602); // Group info
            Out.WriteUInt16(0xB536); // Group id
            Out.WriteByte((byte)Members.Count);
            foreach (Player Plr in Members)
            {
                Out.WriteUInt32(0xA8BF3B0C);
                Out.WriteByte((byte)Plr._Info.ModelId);
                Out.WriteByte((byte)Plr._Info.Race);
                Out.WriteByte((byte)Plr.Level);
                Out.WriteUInt16((ushort)Plr._Info.CareerLine);
                Out.WriteByte(1);
                Out.WriteByte(0); // Will be 1 for at least one member. Perhaps Leader?
                Out.WriteByte(0);
                Out.WriteByte(1); // Online = 1, Offline = 0
                Out.WriteByte((byte)Plr.Name.Length);
                Out.Fill(0, 3);
                Out.WriteStringBytes(Plr.Name);
                Out.WriteByte(0); // Name length. Pet? Target?
                Out.Fill(0, 3);
                //Out.WriteStringBytes(Pet name? Target?);

                Out.WriteByte(0x8B);
                Out.WriteUInt16R((ushort)650); // X ?
                Out.WriteByte(0xD3);
                Out.WriteUInt16R((ushort)650); // y ?
                byte[] data = { 0xC8, 0x50, 0x27, 0x25, 0x05, 0x40, 0x01, 0x02 };
                Out.Write(data);
                
                Out.WriteByte((byte)(Plr._Value.ZoneId*2));
                Out.WriteByte(1);
                Out.WriteByte(1);
 
                Out.WriteByte((byte)Plr.PctHealth);
                Out.WriteByte((byte)Plr.PctAp); // action points
                Out.WriteByte(0);
            }

            SendToGroup(Out);
        }

        public void SendToGroup(PacketOut Out)
        {
            foreach (Player Plr in Members)
            {
                if (Plr != null)
                    Plr.SendPacket(Out);
            }
        }

        public void SetLeader(Player Plr)
        {
            if (Plr == null)
                return;

            Log.Debug("Group", "SetLeader " + Plr.Name);
            Leader = Plr;
            Plr.SendLocalizeString("", GameData.Localized_text.TEXT_YOU_NOW_PARTY_LEADER);
        }

        public int Size()
        {
            return Members.Count;
        }

        public bool IsFull()
        {
            return Members.Count >= 6;
        }

        public void AddMember(Player Plr)
        {
            if (Plr == null)
                return;
           
            if(Members.Contains(Plr))
                return;

            Log.Debug("Group", "AddMember " + Plr.Name);

            if (Plr.GetGroup() != null && Plr.GetGroup() != this)
                Plr.GetGroup().RemoveMember(Plr);

            Members.Add(Plr);

            Plr.SendLocalizeString("", GameData.Localized_text.TEXT_YOU_JOIN_PARTY);

            if (Plr != Leader)
                SendWhosLeader(Plr);

            SendNewMember(Plr);

            Update();
        }

        public void RemoveMember(Player Plr)
        {
            if (Plr == null)
                return;

            Log.Debug("Group", "RemoveMember " + Plr.Name);

            Members.Remove(Plr);
            SendNullGroup(Plr);

            foreach (Player grpPlr in Members)
            {
                if(grpPlr != null)
                    grpPlr.SendLocalizeString(Plr.Name, GameData.Localized_text.TEXT_X_LEFT_PARTY);
            }

            if (Plr == Leader)
                Leader = null;

            Update();
        }

        public void SendNullGroup(Player Plr)
        {
            if (Plr == null)
                return;

            Plr.SendLocalizeString("", GameData.Localized_text.TEXT_YOU_LEFT_PARTY);

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteUInt32(0x06028C11);
            Out.WriteByte(0);
            Plr.SendPacket(Out);
        }

        public void SendWhosLeader(Player Plr)
        {
            if (Leader == null)
                return;

            Plr.SendLocalizeString(Leader.Name, GameData.Localized_text.TEXT_NEW_PARTY_LEADER);
        }

        public void SendNewMember(Player Plr)
        {
	        foreach(Player grpPlr in Members)
	        {
		        if(grpPlr == null )
                    continue;
                grpPlr.SendLocalizeString(Plr.Name, GameData.Localized_text.TEXT_PLAYER_JOIN_PARTY);
	        }
        }

        public void SendMessageToGroup(Player Sender, string Text)
        {
            foreach (Player Plr in Members)
            {
                Plr.SendMessage(Sender, Text, SystemData.ChatLogFilters.CHATLOGFILTERS_GROUP);
            }
        }

        public void UpdatePlayerPosition(Player Plr)
        {
            if(Plr == null)
                return;

            Update();
        }

        public void Delete()
        {
            Log.Debug("Group", "Deleting Group");

            foreach (Player Plr in Members)
            {
                if (Plr == null)
                    continue;

                SendNullGroup(Plr);
                Plr.SetGroup(null);

                if (Plr == Leader)
                    Leader = null;
            }

            Members.Clear();
        }

        public void AddXp(Player Killer, Unit Victim)
        {
            uint activePlayers = 0;

            foreach (Player Plr in Members)
            {
                if (Plr == Killer || Plr.GetDistance(Killer) < 100)
                {
                    activePlayers++;
                }
            }

            foreach (Player Plr in Members)
            {
                if (Plr == Killer || Plr.GetDistance(Killer) < 100)
                {
                    Plr.AddXp(WorldMgr.GenerateXPCount(Plr, Victim) / activePlayers);
                }
            }
        }

        public void AddRenown(Player Killer, Player Victim)
        {
            uint activePlayers = 0;

            foreach (Player Plr in Members)
            {
                if (Plr == Killer || Plr.GetDistance(Killer) < 100)
                {
                    activePlayers++;
                }
            }

            foreach (Player Plr in Members)
            {
                if (Plr == Killer || Plr.GetDistance(Killer) < 100)
                {
                    Plr.AddRenown(WorldMgr.GenerateRenownCount(Plr, Victim) / activePlayers);
                }
            }
        }

        public void AddMoney(uint Money)
        {
            foreach (Player Plr in Members)
            {
                Plr.AddMoney(Money);
            }
        }
    }
}
