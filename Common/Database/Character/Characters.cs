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
    [DataTable(PreCache = false, TableName = "characters", DatabaseName = "Characters")]
    [Serializable]
    public class Character : DataObject
    {
        private UInt32 _CharacterId;
        private string _Name;
        private int _RealmId;
        private int _AccountId;
        private byte _SlotId;
        private byte _ModelId;
        private byte _Career;
        private byte _CareerLine;
        private byte _Realm;
        private int _HeldLeft;
        private byte _Race;
        private byte[] _Traits;
        private byte _Sex;
        public bool FirstConnect;

        public Character()
            : base()
        {

        }

        [DataElement(AllowDbNull=false)]
        public UInt32 CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(Unique = true,AllowDbNull = false,Varchar=255)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int RealmId
        {
            get { return _RealmId; }
            set { _RealmId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte SlotId
        {
            get { return _SlotId; }
            set { _SlotId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte ModelId
        {
            get { return _ModelId; }
            set { _ModelId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Career
        {
            get { return _Career; }
            set { _Career = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CareerLine
        {
            get { return _CareerLine; }
            set { _CareerLine = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Realm
        {
            get { return _Realm; }
            set { _Realm = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int HeldLeft
        {
            get { return _HeldLeft; }
            set { _HeldLeft = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Race
        {
            get { return _Race; }
            set { _Race = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Traits
        {
            get { return System.Text.UTF8Encoding.UTF8.GetString(_Traits); }
            set 
            {
                _Traits = System.Text.UTF8Encoding.UTF8.GetBytes(value); Dirty = true; 
            }
        }

        [DataElement(AllowDbNull = false)]
        public byte Sex
        {
            get { return _Sex; }
            set { _Sex = value; Dirty = true; }
        }

        public byte[] bTraits
        {
            get { return _Traits; }
            set { _Traits = value; }
        }

        public Character_value Value;

        public List<Character_social> Socials;

        public List<Character_tok> Toks;

        public List<Character_quest> Quests;

        public List<Characters_influence> Influences;

        [Relation(LocalField = "CharacterId", RemoteField = "CharacterId", AutoLoad = true, AutoDelete = true)]
        public Character_mail[] Mails;
    }
}
