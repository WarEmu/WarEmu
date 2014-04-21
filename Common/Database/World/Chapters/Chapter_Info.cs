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
    [DataTable(PreCache = false, TableName = "chapter_infos", DatabaseName = "World")]
    [Serializable]
    public class Chapter_Info : DataObject
    {
        [PrimaryKey()]
        public uint Entry;

        [DataElement()]
        public ushort ZoneId;

        [DataElement(Varchar=255)]
        public string Name;

        [DataElement()]
        public uint CreatureEntry;

        [DataElement()]
        public uint InfluenceEntry;

        [DataElement(Varchar = 30)]
        public string Race;

        [DataElement()]
        public uint ChapterRank;

        [DataElement()]
        public ushort PinX;

        [DataElement()]
        public ushort PinY;

        [DataElement()]
        public ushort TokEntry;

        [DataElement()]
        public uint TokExploreEntry;

        public ushort OffX;
        public ushort OffY;
        public UInt32 MaxInflu;
        public List<Chapter_Reward> Rewards;
    }
}