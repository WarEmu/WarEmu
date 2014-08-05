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
    [DataTable(PreCache = false, TableName = "ip_bans", DatabaseName = "Accounts")]
    [Serializable]
    public class Ip_ban : DataObject
    {
        private string _Ip;
        private int _Expire;

        public Ip_ban()
        {
        }

        [DataElement(Unique = true, Varchar = 255)]
        public string Ip
        {
            get { return _Ip; }
            set { _Ip = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int Expire
        {
            get { return _Expire; }
            set
            {
                _Expire = value;
                Dirty = true;
            }
        }
    }
}
