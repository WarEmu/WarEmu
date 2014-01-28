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
//using System.Linq;
using System.Text;

namespace FrameWork
{
    public class CryptKey
    {
        private string _sKey;
        private byte[] _bKey;

        public CryptKey(string Key)
        {
            SetKey(Key);
        }

        public CryptKey(byte[] Key)
        {
            SetKey(Key);
        }

        public void SetKey(string Key)
        {
            _sKey = (string)Key.Clone();
        }

        public void SetKey(byte[] Key)
        {
            _bKey = (byte[])Key.Clone();
        }

        public byte[] GetbKey()
        {
            return (byte[])_bKey.Clone();
        }

        public string GetsKey()
        {
            return (string)_sKey.Clone();
        }
    }

    public interface ICryptHandler
    {
        PacketIn Decrypt(CryptKey Key,byte[] packet);
        byte[] Crypt(CryptKey Key,byte[] packet);

        CryptKey GenerateKey(BaseClient client);
    }
}
