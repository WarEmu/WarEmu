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
using System.Security.Cryptography;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "accounts", DatabaseName = "Accounts")]
    [Serializable]
    public class Account : DataObject
    {
        private int _AccountId;
        private string _Username;
        private string _Password;
        private string _Ip;
        private string _Token;
        private sbyte _GmLevel;
        private int _Banned;
        public long LastCheck;

        public Account()
        {
        }

        [PrimaryKey(AutoIncrement = true)]
        public int AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; Dirty = true; }
        }

        [DataElement(Unique = true, Varchar = 255)]
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                Dirty = true;
            }
        }

        [DataElement(Varchar = 255)]
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                Dirty = true;
            }
        }

        [DataElement(Varchar = 255)]
        public string CryptPassword;

        [DataElement(Varchar = 255)]
        public string Ip
        {
            get { return _Ip; }
            set
            {
                _Ip = value;
                Dirty = true;
            }
        }

        [DataElement(Varchar = 255)]
        public string Token
        {
            get { return _Token; }
            set
            {
                _Token = value;
                Dirty = true;
            }
        }

        [DataElement(AllowDbNull=false)]
        public sbyte GmLevel
        {
            get { return _GmLevel; }
            set
            {
                _GmLevel = value;
                Dirty = true;
            }
        }

        [DataElement(AllowDbNull = false)]
        public int Banned
        {
            get { return _Banned; }
            set
            {
                _Banned = value;
                Dirty = true;
            }
        }

        [DataElement(AllowDbNull = false)]
        public uint InvalidPasswordCount = 0;


        static public string ConvertSHA256(string value)
        {
            SHA256 sha = SHA256.Create();
            byte[] data = sha.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
