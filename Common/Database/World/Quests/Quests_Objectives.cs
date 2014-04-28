/*
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
    public enum Objective_Type
    {
        QUEST_UNKNOWN = 0,
        QUEST_SPEACK_TO = 1,
        QUEST_KILL_MOB = 2,
        QUEST_USE_GO = 3,
        QUEST_GET_ITEM = 4,
        QUEST_KILL_PLAYERS = 5,
        QUEST_PROTECT_UNIT = 6,
    };

    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "quests_objectives", DatabaseName = "World")]
    [Serializable]
    public class Quest_Objectives : DataObject
    {
        [PrimaryKey(AutoIncrement=true)]
        public int Guid;

        [DataElement()]
        public UInt16 Entry;

        [DataElement()]
        public uint ObjType;

        [DataElement()]
        public uint ObjCount;

        [DataElement()]
        public string Description;

        [DataElement()]
        public string ObjID;

        public Quest Quest;
        public Item_Info Item = null;
        public Creature_proto Creature = null;
        public GameObject_proto GameObject = null;
    }
}
