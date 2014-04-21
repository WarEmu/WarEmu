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
    [DataTable(PreCache = false, TableName = "characters_socials", DatabaseName = "Characters")]
    [Serializable]
    public class Character_social : DataObject
    {
        private UInt32 _CharacterId;

        private UInt32 _DistCharacterId;
        private string _DistName;
        private byte _Friend;
        private byte _Ignore;

        [DataElement(AllowDbNull = false)]
        public UInt32 CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 DistCharacterId
        {
            get { return _DistCharacterId; }
            set { _DistCharacterId = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
        public string DistName
        {
            get { return _DistName; }
            set { _DistName = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Friend
        {
            get { return _Friend; }
            set { _Friend = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Ignore
        {
            get { return _Ignore; }
            set { _Ignore = value; Dirty = true; }
        }

        public object Event;

        public T GetEvent<T>()
        {
            return (T)Convert.ChangeType(Event, typeof(T));
        }
    }
}
