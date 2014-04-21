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
    [Flags]
    public enum AbilityFlags : ushort
    {
        StatsBuff = 1,
        Buff = 2,
        Healing = 4,
        Defensive = 8,
        Granted = 16,
        Debuff = 32,
        Channeled = 64,
        Offensive = 128,
        NeedsPet = 256,
        Passive = 512,
        Damaging = 1024,
    }

    [DataTable(PreCache = false, TableName = "Ability_infos", DatabaseName = "World")]
    [Serializable]
    public class Ability_Info : DataObject
    {
        [PrimaryKey()]
        public UInt16 Entry;

        [DataElement()]
        public byte CareerLine;

        [DataElement(Varchar = 255)]
        public string Name;

        [DataElement()]
        public byte MinimumRank;

        [DataElement()]
        public byte MinimumRenown;

        [DataElement()]
        public byte MinRange;

        [DataElement()]
        public byte Range;

        [DataElement()]
        public ushort IconId;

        [DataElement()]
        public string Requirements;
 
        [DataElement(Varchar = 255)]
        public string Specline;

        [DataElement()]
        public ushort CastTime;

        [DataElement()]
        public ushort Cooldown;

        [DataElement()]
        public ushort ReuseTimer;

        [DataElement()]
        public ushort ReuseTimerMax;

        [DataElement()]
        public byte Category;

        [DataElement()]
        public ushort Flags;

        [DataElement()]
        public byte ApCost;

        [DataElement()]
        public byte ApSec;

        [DataElement()]
        public byte PointCost;

        [DataElement()]
        public uint CashCost;

        [DataElement()]
        public byte MinimumCategoryPoints;

        [DataElement()]
        public byte MaximumPurchaseCount;

        [DataElement()]
        public byte RequiredActionCounterCount;

        [DataElement()]
        public uint RequiredTomeEntryID;

        /// <summary>
        /// Custom Values
        /// </summary>

        [DataElement()]
        public string HandlerName;

        [DataElement(AllowDbNull = false)]
        public UInt16 EffectID;

        [DataElement()]
        public byte TargetType;

        [DataElement()]
        public bool GroupMates = false;

        public ushort MaxRange
        {
            get
            {
                return Range;
            }
        }

        public List<Ability_Stats> Stats;
    }
}
