/*
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

using FrameWork;

namespace Common
{
    public class Character_Objectives
    {
        public Character_quest Quest;
        public Quest_Objectives Objective;
        public int ObjectiveID;
        public int _Count;

        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
                Quest.Dirty = true;
            }
        }

        public bool IsDone()
        {
            if (Objective == null)
                return false;
            else
                return Count >= Objective.ObjCount;
        }
    }

    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "characters_quests", DatabaseName = "Characters")]
    [Serializable]
    public class Character_quest : DataObject
    {
        [DataElement(AllowDbNull = false)]
        public UInt32 CharacterId;

        [DataElement(AllowDbNull = false)]
        public UInt16 QuestID;

        [DataElement(AllowDbNull=false)]
        public string Objectives
        {
            get
            {
                string Value = "";
                foreach (Character_Objectives Obj in _Objectives)
                    Value += Obj.ObjectiveID + ":" + Obj.Count + "|";
                return Value;
            }
            set
            {
                if (value.Length <= 0)
                    return;

                string[] Objs = value.Split('|');

                foreach (string Obj in Objs)
                {
                    if (Obj.Length <= 0)
                        continue;

                    int ObjectiveID = int.Parse(Obj.Split(':')[0]);
                    int Count = int.Parse(Obj.Split(':')[1]);

                    Character_Objectives CObj = new Character_Objectives();
                    CObj.Quest = this;
                    CObj.ObjectiveID = ObjectiveID;
                    CObj._Count = Count;
                    _Objectives.Add(CObj);
                }
            }
        }

        [DataElement(AllowDbNull = false)]
        public bool Done;

        public bool IsDone()
        {
            return _Objectives.TrueForAll(obj => obj.IsDone());
        }

        public List<Character_Objectives> _Objectives = new List<Character_Objectives>();

        public Quest Quest;

        public List<byte> SelectedRewards = new List<byte>();
    }
}
