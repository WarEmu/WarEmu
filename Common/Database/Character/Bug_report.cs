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
    [DataTable(PreCache = false, TableName = "bug_report", DatabaseName = "Characters")]
    [Serializable]
    public class Bug_report : DataObject
    {
        private UInt32 _CharacterId;
        private UInt16 _ZoneId;
        private UInt16 _X;
        private UInt16 _Y;
        private UInt32 _Time;
        private byte _Type;
        private byte _Category;
        private string _Message;

        public Bug_report()
            : base()
        {

        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt32 CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt16 ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; Dirty = true; }
        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt16 X
        {
            get { return _X; }
            set { _X = value; Dirty = true; }
        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt16 Y
        {
            get { return _Y; }
            set { _Y = value; Dirty = true; }
        }

        [DataElement(Unique = true, AllowDbNull = false, Varchar = 255)]
        public UInt32 Time
        {
            get { return _Time; }
            set { _Time = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Type
        {
            get { return _Type; }
            set { _Type = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Category
        {
            get { return _Category; }
            set { _Category = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Message
        {
            get { return _Message; }
            set { _Message = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string FieldSting
        {
            get
            {
                string Value = "";
                foreach (KeyValuePair<uint, string> Item in Fields)
                    Value += Item.Key + ":" + Item.Value.Replace(":", " ") + "|";
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

                    string[] FieldInfo = Obj.Split(':');
                    Fields.Add(new KeyValuePair<uint, string>(uint.Parse(FieldInfo[0]), FieldInfo[1]));
                }
                Dirty = true;
            }
        }

        public List<KeyValuePair<uint, string>> Fields = new List<KeyValuePair<uint, string>>();
    }
}