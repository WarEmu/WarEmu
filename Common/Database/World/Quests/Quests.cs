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
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "quests", DatabaseName = "World")]
    [Serializable]
    public class Quest : DataObject
    {
        [PrimaryKey()]
        public UInt16 Entry;

        [DataElement(Varchar=255,AllowDbNull=false)]
        public string Name;

        [DataElement(AllowDbNull = false)]
        public byte Type;

        [DataElement(AllowDbNull = false)]
        public byte Level;

        [DataElement(AllowDbNull = false)]
        public string Description;

        [DataElement(AllowDbNull = false)]
        public string OnCompletionQuest;

        [DataElement(AllowDbNull = false)]
        public string ProgressText;

        [DataElement(AllowDbNull = false)]
        public string Particular;

        [DataElement(AllowDbNull = false)]
        public uint Xp;

        [DataElement(AllowDbNull = false)]
        public uint Gold;

        [DataElement(AllowDbNull = false)]
        public string Given;

        [DataElement(AllowDbNull = false)]
        public string Choice;

        [DataElement(AllowDbNull = false)]
        public byte ChoiceCount;

        [DataElement(AllowDbNull = false)]
        public UInt16 PrevQuest;

        public List<Quest_Objectives> Objectives = new List<Quest_Objectives>();
        public Dictionary<Item_Info, uint> Rewards = new Dictionary<Item_Info, uint>();
    }
}
