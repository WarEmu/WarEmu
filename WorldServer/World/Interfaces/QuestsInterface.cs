/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
using GameData;

namespace WorldServer
{
    public class QuestsInterface : BaseInterface
    {
        public uint Entry
        {
            get
            {
                if (_Owner == null)
                    return 0;

                if (_Owner.IsCreature())
                    return _Owner.GetCreature().Entry;
               
                return 0;
            }
        }

        #region Npc

        public bool HasQuestStarter(UInt16 QuestID)
        {
            return WorldMgr.GetStartQuests(Entry).Find(info => info.Entry == QuestID) != null;
        }

        public bool hasQuestFinisher(UInt16 QuestID)
        {
            List<Quest> Quests = WorldMgr.GetFinishersQuests(Entry);
            if(Quests != null)
                return WorldMgr.GetFinishersQuests(Entry).Find(info => info.Entry == QuestID) != null;

            return false;
        }

        public bool CreatureHasQuestToComplete(Player Plr)
        {
            if (Entry == 0)
                return false;

            List<Quest> Finisher = WorldMgr.GetFinishersQuests(Entry);
            if (Finisher == null)
                return false;

            return  Finisher.Find(q => Plr.QtsInterface.CanEndQuest(q)) != null;
        }

        public bool CreatureHasQuestToAchieve(Player Plr)
        {
            if (Entry == 0)
                return false;

            List<Quest> Finisher = WorldMgr.GetFinishersQuests(Entry);
            if (Finisher == null)
                return false;

            foreach (Quest Q in Finisher)
            {
                Character_quest CQ = Plr.QtsInterface.GetQuest(Q.Entry);
                if (CQ != null && !CQ.Done && CQ.IsDone())
                    return true;
            }

            return false;
        }

        public bool CreatureHasStartQuest(Player Plr)
        {
            if (Entry == 0)
                return false;

            List<Quest> Starter = WorldMgr.GetStartQuests(Entry);
            if (Starter == null)
                return false;

            return Starter.Find(q => Plr.QtsInterface.CanStartQuest(q)) != null;
        }

        public void HandleInteract(Player Plr, Creature Crea, InteractMenu Menu)
        {
            if(Entry == 0)
                return;

            List<Quest> Starter = Crea.Spawn.Proto.StartingQuests;
            List<Quest> Finisher = Crea.Spawn.Proto.FinishingQuests;
            List<Quest> InProgress = Starter != null ? Starter.FindAll(info => Plr.QtsInterface.HasQuest(info.Entry) && !Plr.QtsInterface.HasDoneQuest(info.Entry)) : null;

            string Text = WorldMgr.GetCreatureText(Entry);

            if (Starter == null && Finisher == null && Text.Length <= 0 && InProgress == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
            Out.WriteByte(0);
            Out.WriteUInt16(_Owner.Oid);
            Out.Fill(0, 3);
            Out.WriteByte(0x60);
            Out.WriteUInt32(0);
            Out.WriteUInt16(Plr.Oid);

            if (Starter != null)
            {
                List<Quest> Starts = Starter.FindAll(q => Plr.QtsInterface.CanStartQuest(q) );

                Out.WriteByte((byte)Starts.Count);
                foreach (Quest Q in Starts)
                {
                    Out.WriteByte(0);
                    Out.WriteUInt16(Q.Entry);
                    Out.WriteUInt16(0);
                    Out.WritePascalString(Q.Name);
                }
            }
            else
                Out.WriteByte(0);

            if (Finisher != null)
            {
                List<Quest> Finishs = Finisher.FindAll(q => Plr.QtsInterface.CanEndQuest(q));

                Out.WriteByte((byte)Finishs.Count);
                foreach (Quest Q in Finishs)
                {
                    Out.WriteByte(0);
                    Out.WriteUInt16(Q.Entry);
                    Out.WritePascalString(Q.Name);
                }
            }
            else if (InProgress != null)
            {
                Out.WriteByte((byte)InProgress.Count);
                foreach (Quest Q in InProgress)
                {
                    Out.WriteByte(0);
                    Out.WriteUInt16(Q.Entry);
                    Out.WritePascalString(Q.Name);
                }
            }
            else
                Out.WriteByte(0);

            Out.WritePascalString(Text);
            Out.WriteByte(0);

            Plr.SendPacket(Out);
        }

        #endregion

        #region Players

        public Dictionary<ushort, Character_quest> _Quests = new Dictionary<ushort, Character_quest>();

        public void Load(List<Character_quest> Quests)
        {
            if (Quests == null)
                return;

            foreach (Character_quest Quest in Quests)
            {
                Quest.Quest = WorldMgr.GetQuest(Quest.QuestID);
                if (Quest.Quest == null)
                    continue;

                foreach (Character_Objectives Obj in Quest._Objectives)
                    Obj.Objective = WorldMgr.GetQuestObjective(Obj.ObjectiveID);

                if (!_Quests.ContainsKey(Quest.QuestID))
                    _Quests.Add(Quest.QuestID, Quest);
            }
        }

        public override void Save()
        {
            foreach (KeyValuePair<ushort, Character_quest> Kp in _Quests)
                CharMgr.Database.SaveObject(Kp.Value);

            // Lock? Threadsafe?
            CharMgr._Chars[_Owner.GetPlayer().CharacterId].Quests = _Quests.Values.ToList<Character_quest>();
        }

        public bool HasQuest(UInt16 QuestID)
        {
            if (QuestID == 0)
                return true;

            return _Quests.ContainsKey(QuestID);
        }

        public bool HasFinishQuest(UInt16 QuestID)
        {
            if (QuestID == 0)
                return true;

            if (!HasQuest(QuestID))
                return false;

            return GetQuest(QuestID).IsDone();
        }

        public bool HasDoneQuest(UInt16 QuestID)
        {
            if (QuestID == 0)
                return true;

            if (!HasQuest(QuestID))
                return false;

            return GetQuest(QuestID).Done;
        }

        public Character_quest GetQuest(UInt16 QuestID)
        {
            Character_quest Quest;
            _Quests.TryGetValue(QuestID, out Quest);
            return Quest;
        }

        public bool CanStartQuest(Quest Quest)
        {
            if(GetPlayer() == null)
                return false;

            if (Quest == null)
                return false;

            // TODO : Fixe Starting Quests
            if (HasQuest(Quest.Entry) || Quest.Level > (GetPlayer().Level+1) || (Quest.PrevQuest != 0 && !HasDoneQuest(Quest.PrevQuest)))
                return false;

            return true;
        }

        public bool CanEndQuest(Quest Quest)
        {
            if (GetPlayer() == null)
                return false;

            if (Quest == null)
                return false;

            if (!HasQuest(Quest.Entry) || !HasFinishQuest(Quest.Entry) || HasDoneQuest(Quest.Entry))
                return false;

            return true;
        }

        public bool AcceptQuest(UInt16 QuestID)
        {
            return AcceptQuest(WorldMgr.GetQuest(QuestID));
        }

        public bool AcceptQuest(Quest Quest)
        {
            if (Quest == null)
                return false;

            if (!CanStartQuest(Quest))
                return false;

            Character_quest CQuest = new Character_quest();
            CQuest.QuestID = Quest.Entry;
            CQuest.Done = false;
            CQuest.CharacterId = GetPlayer().CharacterId;
            CQuest.Quest = Quest;

            foreach(Quest_Objectives QObj in Quest.Objectives)
            {
                Character_Objectives CObj = new Character_Objectives();
                CObj.Quest = CQuest;
                CObj._Count = 0;
                CObj.Objective = QObj;
                CObj.ObjectiveID = QObj.Guid;
                CQuest._Objectives.Add(CObj);
            }

            CharMgr.Database.AddObject(CQuest);
            _Quests.Add(Quest.Entry, CQuest);

            SendQuestState(Quest, QuestCompletion.QUESTCOMPLETION_OFFER);

            // This will make objects lootable if they contain a quest object.
            updateObjects();

            _Owner.EvtInterface.Notify(EventName.ON_ACCEPT_QUEST, _Owner, CQuest);
            return true;
        }

        public void DeclineQuest(UInt16 QuestID)
        {
            Character_quest Quest = GetQuest(QuestID);
            if (Quest == null)
                return;

            _Quests.Remove(Quest.QuestID);
            SendQuestState(Quest.Quest, QuestCompletion.QUESTCOMPLETION_ABANDONED);
            CharMgr.Database.DeleteObject(Quest);

            // This will make objects unlootable if they were lootable because of a quest.
            updateObjects();

            // Update quest givers around
            foreach (Object Obj in _Owner._ObjectRanged)
            {
                if (Obj.IsCreature())
                    Obj.GetCreature().SendMeTo(_Owner.GetPlayer());
            }

            _Owner.EvtInterface.Notify(EventName.ON_ACCEPT_QUEST, _Owner, Quest);
        }

        public bool DoneQuest(UInt16 QuestID)
        {
             Character_quest Quest = GetQuest(QuestID);

            if (Quest == null || !Quest.IsDone())
                return false;

            Player Plr = GetPlayer();

            Dictionary<Item_Info, uint> Choices = GenerateRewards(Quest.Quest, Plr);

            UInt16 FreeSlots = Plr.ItmInterface.GetTotalFreeInventorySlot();
            if (FreeSlots < Quest.SelectedRewards.Count)
            {
                Plr.SendLocalizeString("", Localized_text.TEXT_OVERAGE_CANT_SALVAGE);
                return false;
            }

            foreach (Quest_Objectives Obj in Quest.Quest.Objectives)
            {
                if ((Objective_Type)Obj.ObjType == Objective_Type.QUEST_GET_ITEM)
                {
                    if (Obj.Item != null)
                    {
                        Plr.ItmInterface.RemoveAllItems(Obj.Item.Entry);
                    }
                }
            }

            byte num = 0;
            foreach (KeyValuePair<Item_Info, uint> Kp in Choices)
            {
                if (Quest.SelectedRewards.Contains(num))
                {
                    Plr.ItmInterface.CreateItem(Kp.Key, (ushort)Kp.Value);
                }
                ++num;
            }

            Plr.AddXp(Quest.Quest.Xp);
            Plr.AddMoney(Quest.Quest.Gold);

            Quest.Done = true;
            Quest.Dirty = true;
            Quest.SelectedRewards.Clear();

            SendQuestState(Quest.Quest, QuestCompletion.QUESTCOMPLETION_DONE);
            CharMgr.Database.SaveObject(Quest);

            _Owner.EvtInterface.Notify(EventName.ON_DONE_QUEST, _Owner, Quest);
            return true;
        }

        public void FinishQuest(Quest Quest)
        {
            if (Quest == null)
                return;

            if (!HasFinishQuest(Quest.Entry))
                return;
        }

        public void HandleEvent(Objective_Type Type, uint Entry, int Count, bool Group = false)
        {
            if (!Group && _Owner.IsPlayer() && _Owner.GetPlayer().GetGroup() != null)
            {
                Group Current = _Owner.GetPlayer().GetGroup();

                lock (Current.Members)
                {
                    foreach (Player SubPlayer in Current.Members)
                        if (SubPlayer != _Owner && SubPlayer.GetDistance(_Owner) < 150)
                        {
                            SubPlayer.QtsInterface.HandleEvent(Type, Entry, Count, true);
                        }
                }
            }

            foreach (KeyValuePair<ushort, Character_quest> QuestKp in _Quests)
            {
                foreach (Character_Objectives Objective in QuestKp.Value._Objectives)
                {
                    if (Objective.Objective.ObjType == (uint)Type && !Objective.IsDone())
                    {
                        bool CanAdd = false;
                        int NewCount = Objective.Count;

                        if (Type == Objective_Type.QUEST_SPEACK_TO || Type == Objective_Type.QUEST_KILL_MOB || Type == Objective_Type.QUEST_PROTECT_UNIT)
                        {
                            if (Objective.Objective.Creature != null && Entry == Objective.Objective.Creature.Entry)
                            {
                                CanAdd = true;
                                NewCount += Count;
                            }
                        }
                        else if (Type == Objective_Type.QUEST_GET_ITEM)
                        {
                            if (Objective.Objective.Item != null && Entry == Objective.Objective.Item.Entry)
                            {
                                CanAdd = true;
                                NewCount = _Owner.GetPlayer().ItmInterface.GetItemCount(Entry);
                            }
                        }
                        else if (Type == Objective_Type.QUEST_USE_GO)
                        {
                            if (Objective.Objective.GameObject != null && Entry == Objective.Objective.GameObject.Entry)
                            {
                                CanAdd = true;
                                NewCount += Count;
                            }
                        }
                        else if (Type == Objective_Type.QUEST_UNKNOWN)
                        {
                            if (Objective.Objective.Guid == Entry)
                            {
                                CanAdd = true;
                                NewCount += Count;
                            }
                        }

                        if (CanAdd)
                        {
                            Objective.Count = NewCount;
                            QuestKp.Value.Dirty = true;
                            SendQuestUpdate(QuestKp.Value);
                            CharMgr.Database.SaveObject(QuestKp.Value);

                            if (Objective.IsDone())
                            {
                                Creature Finisher;

                                foreach (Object Obj in _Owner._ObjectRanged)
                                {
                                    if (Obj.IsCreature())
                                    {
                                        Finisher = Obj.GetCreature();
                                        if (WorldMgr.HasQuestToFinish(Finisher.Entry, QuestKp.Value.Quest.Entry))
                                            Finisher.SendMeTo(_Owner.GetPlayer());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SelectRewards(UInt16 QuestID, byte num)
        {
            Character_quest Quest = GetQuest(QuestID);
            if (Quest == null || !Quest.IsDone())
                return;

            if (num > 0)
                --num;

            Log.Info("SelectRewards", "Selection de la recompence : " + num);
            Quest.SelectedRewards.Add(num);
        }

        #endregion

        static public void BuildQuestInfo(PacketOut Out, Player Plr, Quest Q)
        {
            BuildQuestHeader(Out, Q, true);

            BuildQuestRewards(Out, Plr, Q);

            BuildObjectives(Out, Q.Objectives);

            Out.WriteByte(0);
        }
        static public void BuildQuestHeader(PacketOut Out, Quest Q, bool Particular)
        {
            Out.WritePascalString(Q.Name);
            Out.WriteUInt16((UInt16)Q.Description.Length);
            Out.WriteStringBytes(Q.Description);
            if (Particular)
            {
                Out.WriteUInt16((UInt16)Q.Particular.Length);
                Out.WriteStringBytes(Q.Particular);
            }
            Out.WriteByte(1);
            Out.WriteUInt32(Q.Gold);
            Out.WriteUInt32(Q.Xp);

        }

        static public void BuildQuestInProgress(PacketOut Out, Quest Q, bool Particular)
        {
            Out.WritePascalString(Q.Name);

            if (Q.ProgressText.Length > 0)
            {
                Out.WriteUInt16((UInt16)Q.ProgressText.Length);
                Out.WriteStringBytes(Q.ProgressText);
            }
            else
            {
                Out.WriteUInt16((UInt16)Q.Particular.Length);
                Out.WriteStringBytes(Q.Particular);
            }

            Out.WriteByte(1);
        }

        static public void BuildQuestComplete(PacketOut Out, Quest Q, bool Particular)
        {
            Out.WritePascalString(Q.Name);

            if (Q.OnCompletionQuest.Length > 0)
            {
                Out.WriteUInt16((UInt16)Q.OnCompletionQuest.Length);
                Out.WriteStringBytes(Q.OnCompletionQuest);
            }
            else
            {
                Out.WriteUInt16((UInt16)Q.Description.Length);
                Out.WriteStringBytes(Q.Description);
            }

            if (Particular)
            {
                Out.WriteUInt16((UInt16)Q.Particular.Length);
                Out.WriteStringBytes(Q.Particular);
            }
            Out.WriteByte(1);
            Out.WriteUInt32(Q.Gold);
            Out.WriteUInt32(Q.Xp);
        }
        static public void BuildQuestRewards(PacketOut Out, Player Plr, Quest Q)
        {
            Dictionary<Item_Info, uint> Choices = GenerateRewards(Q, Plr);

            Out.WriteByte(Math.Min(Q.ChoiceCount,(byte)Choices.Count));
            Out.WriteByte(0);
            Out.WriteByte((byte)Choices.Count);

            foreach (KeyValuePair<Item_Info, uint> Kp in Choices)
                Item.BuildItem(ref Out, null, Kp.Key, 0, (ushort)Kp.Value);
        }
        static public void BuildQuestInteract(PacketOut Out,UInt16 QuestID, UInt16 SenderOid, UInt16 ReceiverOid)
        {
            Out.WriteUInt16(QuestID);
            Out.WriteUInt16(0);

            Out.WriteUInt16(SenderOid);
            Out.WriteUInt16(0);

            Out.WriteUInt16(ReceiverOid);
        }

        public void BuildQuest(UInt16 QuestID, Player Plr)
        {
            Quest Q = WorldMgr.GetQuest(QuestID);
            if (Q == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
            Out.WriteByte(1);
            Out.WriteByte(1);

            BuildQuestInteract(Out, Q.Entry, _Owner.Oid, Plr.Oid);

            Out.WriteUInt16(0);

            BuildQuestInfo(Out, Plr, Q);

            Plr.SendPacket(Out);
        }

        public void BuildQuest(PacketOut Out, Quest Q)
        {
            Out.WriteByte(Q.ChoiceCount);
            Out.WriteByte(0);
        }

        static public void BuildObjectives(PacketOut Out, List<Quest_Objectives> Objs)
        {
            Out.WriteByte((byte)Objs.Count);

            foreach (Quest_Objectives Objective in Objs)
            {
                Out.WriteByte((byte)Objective.ObjCount);
                Out.WritePascalString(Objective.Description);
            }
        }

        static public void BuildObjectives(PacketOut Out, List<Character_Objectives> Objs)
        {
            Out.WriteByte((byte)Objs.Count);

            foreach (Character_Objectives Objective in Objs)
            {
                Out.WriteByte((byte)Objective.Count);
                Out.WriteByte((byte)Objective.Objective.ObjCount);
                Out.WriteUInt16(0);
                Out.WritePascalString(Objective.Objective.Description);
            }
        }

        public void SendQuest(ushort QuestID)
        {
            Character_quest CQuest = GetQuest(QuestID);
            SendQuest(CQuest);
        }

        public void SendQuests()
        {
            List<Character_quest> Quests = _Quests.Values.ToList<Character_quest>().FindAll(q => q.Done == false);

            PacketOut Out = new PacketOut((byte)Opcodes.F_QUEST_LIST);
            Out.WriteByte((byte)Quests.Count);
            foreach (Character_quest Quest in Quests)
            {
                Out.WriteUInt16(Quest.QuestID);
                Out.WriteByte(0);
                Out.WritePascalString(Quest.Quest.Name);
                Out.WriteByte(0);
            }

            GetPlayer().SendPacket(Out);
        }

        public void SendQuest(Character_quest CQuest)
        {
            if (CQuest == null)
            {
                Log.Error("QuestsInterface", "SendQuest CQuest == null");
                return;
            }

            PacketOut Packet = new PacketOut((byte)Opcodes.F_QUEST_INFO);
            Packet.WriteUInt16(CQuest.QuestID);
            Packet.WriteByte(0);
            BuildQuestHeader(Packet, CQuest.Quest, true);

            Dictionary<Item_Info, uint> Rewards = GenerateRewards(CQuest.Quest, GetPlayer());

            Packet.WriteByte(CQuest.Quest.ChoiceCount);
            Packet.WriteByte(0);
            Packet.WriteByte((byte)Rewards.Count);

            foreach (KeyValuePair<Item_Info, uint> Kp in Rewards)
            {
                Item.BuildItem(ref Packet, null, Kp.Key, 0, (ushort)Kp.Value);
            }

            Packet.WriteByte(0);

            BuildObjectives(Packet, CQuest._Objectives);

            Packet.WriteByte(1);

            Packet.WritePascalString(CQuest.Quest.Name);
            Packet.WritePascalString("Return to your giver");

            Packet.WriteUInt16(0x006A);
            Packet.WriteUInt16(0x046D);
            Packet.WriteUInt16(0x4D9E);
            Packet.WriteUInt16(0xCB65);

            Packet.Fill(0, 18);

            GetPlayer().SendPacket(Packet);
        }

        public void SendQuestDoneInfo(Player Plr, UInt16 QuestID)
        {
            Character_quest Quest = Plr.QtsInterface.GetQuest(QuestID);

            if (Quest == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
            Out.WriteByte(3);
            Out.WriteByte(0);

            BuildQuestInteract(Out, Quest.QuestID, _Owner.Oid, Plr.Oid);

            BuildQuestComplete(Out, Quest.Quest, false);

            BuildQuestRewards(Out, Plr, Quest.Quest);

            Plr.SendPacket(Out);
        }

        public void SendQuestInProgressInfo(Player Plr, UInt16 QuestID)
        {
            Character_quest Quest = Plr.QtsInterface.GetQuest(QuestID);

            if (Quest == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
            Out.WriteByte(2);
            Out.WriteByte(1);

            BuildQuestInteract(Out, Quest.QuestID, _Owner.Oid, Plr.Oid);

            BuildQuestInProgress(Out, Quest.Quest, false);

            Plr.SendPacket(Out);
        }

        public void SendQuestState(Quest Quest,QuestCompletion State)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_QUEST_LIST_UPDATE);
            Out.WriteUInt16(Quest.Entry);

            if (State == QuestCompletion.QUESTCOMPLETION_ABANDONED || State == QuestCompletion.QUESTCOMPLETION_DONE)
                Out.WriteByte(0);
            else
                Out.WriteByte(1);

            Out.WriteByte((byte)(State == QuestCompletion.QUESTCOMPLETION_DONE ? 1 : 0));

            Out.WriteUInt32(0x0000FFFF);
            Out.WritePascalString(Quest.Name);
            Out.WriteByte(0);
            GetPlayer().SendPacket(Out);
        }

        public void SendQuestUpdate(Character_quest Quest)
        {
            if (GetPlayer() == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_QUEST_UPDATE);
            Out.WriteUInt16(Quest.QuestID);
            Out.WriteByte(Convert.ToByte(Quest.IsDone()));
            Out.WriteByte((byte)Quest._Objectives.Count);
            foreach (Character_Objectives Obj in Quest._Objectives)
            {
                Out.WriteByte((byte)Obj.Count);
            }
            Out.WriteUInt16(0);
            GetPlayer().SendPacket(Out);
        }

        static public Dictionary<Item_Info,uint> GenerateRewards(Quest Q, Player Plr)
        {
            Dictionary<Item_Info,uint> Rewards = new Dictionary<Item_Info,uint>();

            foreach (KeyValuePair<Item_Info, uint> Kp in Q.Rewards)
                if (ItemsInterface.CanUse(Kp.Key, Plr, true, false, false, false, false))
                    Rewards.Add(Kp.Key, Kp.Value);

            return Rewards;
        }

        // For quests which require you to loot GameObjects this will update any objects
        // around you and make them lootable if they have items you need for a quest.
        // Notes: We could minimise the amount of SendMeTo's by checking if object are already
        // flagged and unflag them or unflagged and need flagging. However it isnt possible
        // to see if its already been flagged at the moment.
        public void updateObjects()
        {
            GameObject GameObject;

            foreach (Object Obj in _Owner._ObjectRanged)
            {
                if (Obj.IsGameObject())
                {
                    GameObject = Obj.GetGameObject();
                    //Loot Loots = LootsMgr.GenerateLoot(GameObject, _Owner.GetPlayer());
                    //if (Loots != null && Loots.IsLootable())
                    GameObject.SendMeTo(_Owner.GetPlayer());
                }
            }
        }
    }
}
