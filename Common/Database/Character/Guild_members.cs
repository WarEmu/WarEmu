/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
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
    [DataTable(PreCache = false, TableName = "guild_members", DatabaseName = "Characters")]
    [Serializable]
    public class Guild_member : DataObject
    {
        private UInt32 _GuildId;
        private UInt32 _CharacterId;
        private UInt32 _RankId;
        private string _MemberNote;
        private string _OfficerNote;

        public Guild_member()
            : base()
        {

        }

        [DataElement(AllowDbNull = false)]
        public UInt32 GuildId
        {
            get { return _GuildId; }
            set { _GuildId = value; Dirty = true; }
        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt32 CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 RankId
        {
            get { return _RankId; }
            set { _RankId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string MemberNote
        {
            get { return _MemberNote; }
            set { _MemberNote = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string OfficerNote
        {
            get { return _OfficerNote; }
            set { _OfficerNote = value; Dirty = true; }
        }

        public Character Member;
        public Guild_rank Rank;
    }
}