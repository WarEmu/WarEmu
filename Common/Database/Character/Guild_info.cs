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
    [DataTable(PreCache = false, TableName = "guild_info", DatabaseName = "Characters")]
    [Serializable]
    public class Guild_info : DataObject
    {
        private UInt32 _GuildId;
        private string _Name;
        private UInt32 _Level;
        private int _LeaderId;
        private int _CreateDate;
        private string _Motd;
        private string _AboutUs;

        public Guild_info()
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
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 Level
        {
            get { return _Level; }
            set { _Level = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int LeaderId
        {
            get { return _LeaderId; }
            set { _LeaderId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Motd
        {
            get { return _Motd; }
            set { _Motd = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string AboutUs
        {
            get { return _AboutUs; }
            set { _AboutUs = value; Dirty = true; }
        }

        public Guild_member Leader; 
        public List<Guild_member> Members;
        public List<Guild_rank> Ranks;
        public List<Guild_log> Logs;
    }
}
