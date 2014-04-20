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
using System.Text;
using System.IO;

namespace FrameWork
{
    public enum PackStruct
    {
        OpcodeAndSize = 0x01,
        SizeAndOpcode = 0x02,
    };

    public class PacketOut : MemoryStream
    {
        static public int SizeLen = sizeof(UInt32); // 4 byte
        static public bool OpcodeInLen = false;  // Opcode on Size ?
        static public bool SizeReverse = false;   // Reversed Size ?
        static public bool OpcodeReverse = false; // Reversed Opcode ?
        static public int InterOpcodeSizeCount = 0; // Space Size 
        static public bool SizeInLen = true; // Size in Size ?
        static public PackStruct Struct = PackStruct.SizeAndOpcode;

        public int OpcodeLen = sizeof(UInt16); // 2 bytes
        public UInt64 Opcode = 0;

        protected PacketOut()
        {
        }

        public PacketOut(byte opcode)
            : base(sizeof(byte) + SizeLen)
        {
            Opcode = opcode;
            OpcodeLen = sizeof(byte);

            if (Struct == PackStruct.SizeAndOpcode) WriteSize();
            else if (Struct == PackStruct.OpcodeAndSize) WriteByte(opcode);

            for (int i = 0; i < InterOpcodeSizeCount; ++i)
                WriteByte(0);

            if (Struct == PackStruct.SizeAndOpcode) WriteByte(opcode);
            else if (Struct == PackStruct.OpcodeAndSize) WriteSize();
        }

        public PacketOut(UInt16 opcode)
            : base(sizeof(UInt16) + SizeLen)
        {
            if (Struct == PackStruct.SizeAndOpcode) WriteSize();
            else if (Struct == PackStruct.OpcodeAndSize)
                if (!OpcodeReverse) WriteUInt16(opcode);
                else WriteUInt16R(opcode);

            for (int i = 0; i < InterOpcodeSizeCount; ++i)
                WriteByte(0);

            if (Struct == PackStruct.SizeAndOpcode) 
                if (!OpcodeReverse) WriteUInt16(opcode);
                else WriteUInt16R(opcode);
            else if (Struct == PackStruct.OpcodeAndSize) WriteSize();
        }

        public PacketOut(UInt32 opcode)
            : base(sizeof(UInt32) + SizeLen)
        {
            if (Struct == PackStruct.SizeAndOpcode) WriteSize();
            else if (Struct == PackStruct.OpcodeAndSize)
                if (!OpcodeReverse) WriteUInt32(opcode);
                else WriteUInt32R(opcode);

            for (int i = 0; i < InterOpcodeSizeCount; ++i)
                WriteByte(0);

            if (Struct == PackStruct.SizeAndOpcode)
                if (!OpcodeReverse) WriteUInt32(opcode);
                else WriteUInt32R(opcode);
            else if (Struct == PackStruct.OpcodeAndSize) WriteSize();
        }

        public PacketOut(UInt64 opcode)
            : base(sizeof(UInt64) + SizeLen)
        {
            if (Struct == PackStruct.SizeAndOpcode) WriteSize();
            else if (Struct == PackStruct.OpcodeAndSize)
                if (!OpcodeReverse) WriteUInt64(opcode);
                else WriteUInt64R(opcode);

            for (int i = 0; i < InterOpcodeSizeCount; ++i)
                WriteByte(0);

            if (Struct == PackStruct.SizeAndOpcode)
                if (!OpcodeReverse) WriteUInt64(opcode);
                else WriteUInt64R(opcode);
            else if (Struct == PackStruct.OpcodeAndSize) WriteSize();
        }

        public void WriteSize()
        {
            for (int i = 0; i < SizeLen; ++i)
                WriteByte(0);
        }

        public virtual ulong WritePacketLength()
        {
            if (Struct == PackStruct.OpcodeAndSize)
                Position = OpcodeLen + InterOpcodeSizeCount;
            else if (Struct == PackStruct.SizeAndOpcode)
                Position = 0;
            else
                Position = 0;


            long size = OpcodeInLen == false ? (Length - OpcodeLen) : Length;
            if (!SizeInLen) size -= SizeLen;

            switch (SizeLen)
            {
                case sizeof(byte):
                    WriteByte((byte)(size));
                    break;

                case sizeof(UInt16):
                    if (!SizeReverse) WriteUInt16((UInt16)(size));
                    else WriteUInt16R((UInt16)size);
                    break;

                case sizeof(UInt32):
                    if (!SizeReverse) WriteUInt32((UInt16)(size));
                    else WriteUInt32R((UInt16)size);
                    break;

                case sizeof(UInt64):
                    if (!SizeReverse) WriteUInt32((UInt16)(size));
                    else WriteUInt32R((UInt16)size);
                    break;
            }

            Capacity = (int)Length;

            return (ulong)(size);
        }

        #region IPacket Members

        #endregion

        public virtual void Write(byte[] val)
        {
            Write(val, 0, val.Length);
        }

        public virtual void WriteUInt16(ushort val)
        {
            WriteByte((byte)(val >> 8));
            WriteByte((byte)(val & 0xff));
        }
        public virtual void WriteUInt16R(ushort val)
        {
            WriteByte((byte)(val & 0xff));
            WriteByte((byte)(val >> 8));
        }

        public virtual void WriteInt16(Int16 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = b.Length; i > 0; --i)
                WriteByte(b[i-1]);
        }

        public virtual void WriteInt16R(Int16 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = 0; i < b.Length; ++i)
                WriteByte(b[i]);
        }

        public virtual void WriteUInt32(uint val)
        {
            WriteByte((byte)(val >> 24));
            WriteByte((byte)((val >> 16) & 0xff));
            WriteByte((byte)((val & 0xffff) >> 8));
            WriteByte((byte)((val & 0xffff) & 0xff));
        }
        public virtual void WriteUInt32R(uint val)
        {
            WriteByte((byte)((val & 0xffff) & 0xff));
            WriteByte((byte)((val & 0xffff) >> 8));
            WriteByte((byte)((val >> 16) & 0xff));
            WriteByte((byte)(val >> 24));
        }

        public virtual void WriteInt32(Int32 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = 0; i < b.Length; ++i)
                WriteByte(b[i]);
        }
        public virtual void WriteInt32R(Int32 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = b.Length; i > 0; --i)
                WriteByte(b[i - 1]);
        }

        public virtual void WriteUInt64(ulong val)
        {
            WriteByte((byte)(val >> 56));
            WriteByte((byte)((val >> 48) & 0xff));
            WriteByte((byte)((val >> 40) & 0xff));
            WriteByte((byte)((val >> 32) & 0xff));
            WriteByte((byte)((val >> 24) & 0xff));
            WriteByte((byte)((val >> 16) & 0xff));
            WriteByte((byte)((val >> 8) & 0xff));
            WriteByte((byte)(val & 0xff));
        }
        public virtual void WriteUInt64R(ulong val)
        {
            WriteByte((byte)(val & 0xff));
            WriteByte((byte)((val >> 8) & 0xff));
            WriteByte((byte)((val >> 16) & 0xff));
            WriteByte((byte)((val >> 24) & 0xff));
            WriteByte((byte)((val >> 32) & 0xff));
            WriteByte((byte)((val >> 40) & 0xff));
            WriteByte((byte)((val >> 48) & 0xff));
            WriteByte((byte)(val >> 56));
        }

        public virtual void WriteInt64(Int64 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = 0; i < b.Length; ++i)
                WriteByte(b[i]);
        }
        public virtual void WriteInt64R(Int64 val)
        {
            byte[] b = BitConverter.GetBytes(val);

            for (int i = 0; i < b.Length; ++i)
                WriteByte(b[i]);
        }

        public virtual void WriteFloat(float val)
        {
            foreach (Byte b in BitConverter.GetBytes(val))
                WriteByte(b);
        }

        public virtual byte GetChecksum()
        {
            byte val = 0;
            byte[] buf = GetBuffer();

            for (int i = 0; i < Position - 6; ++i)
                val += buf[i + 8];

            return val;
        }

        public virtual void Fill(byte val, int num)
        {
            for (int i = 0; i < num; ++i)
                WriteByte(val);
        }

        public virtual void WritePascalString(string str)
        {
            if (str == null || str.Length <= 0)
            {
                WriteByte(0);
                return;
            }

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(str);
            WriteByte((byte)bytes.Length);
            Write(bytes, 0, bytes.Length);
        }

        public virtual void WriteStringToZero(string str)
        {
            if (str == null || str.Length <= 0)
            {
                WriteByte(1);
            }
            else
            {
                byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(str);
                WriteByte((byte)(bytes.Length + 1));
                Write(bytes, 0, bytes.Length);
            }

            WriteByte(0);
        }

        public virtual void WriteString(string str)
        {
            WriteUInt32((UInt32)str.Length);
            WriteStringBytes(str);
        }
        public virtual void WriteStringBytes(string str)
        {
            if (str.Length <= 0)
                return;

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(str);
            Write(bytes, 0, bytes.Length);
        }
        public virtual void WriteString(string str, int maxlen)
        {
            if (str.Length <= 0)
                return;

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(str);
            Write(bytes, 0, bytes.Length < maxlen ? bytes.Length : maxlen);
        }
        public virtual void WriteUnicodeString(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);//each char becomes 2 bytes

            for (int i = 0; i < data.Length; ++i)
                WriteByte(data[i]);

            WriteByte(0x00);//null terminated string
            WriteByte(0x00);//
        }
        public virtual void WriteUnicodeString(string str, int maxlen)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);//each char becomes 2 bytes
            int i = 0;
            for (; i < data.Length && i < maxlen; ++i)
                WriteByte(data[i]);

            if (i < maxlen)
                for (; i < maxlen; ++i)
                    WriteByte(0);

            WriteByte(0x00);//null terminated string
            WriteByte(0x00);//
        }

        public virtual void FillString(string str, int len)
        {
            long pos = Position;

            Fill(0x0, len);

            if (str == null)
                return;

            Position = pos;

            if (str.Length <= 0)
            {
                Position = pos + len;
                return;
            }

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(str);
            Write(bytes, 0, len > bytes.Length ? bytes.Length : len);
            Position = pos + len;
        }

        public virtual void WriteVector2(Vector2 Vector)
        {
            WriteFloat(Vector.X);
            WriteFloat(Vector.Y);
        }

        public virtual void WriteVector3(Vector3 Vector)
        {
            WriteFloat(Vector.X);
            WriteFloat(Vector.Y);
            WriteFloat(Vector.Z);
        }

        public virtual void WriteQuaternion(Quaternion Quat)
        {
            WriteFloat(Quat.X);
            WriteFloat(Quat.Y);
            WriteFloat(Quat.Z);
            WriteFloat(Quat.W);
        }

        public virtual void WriteHexStringBytes(string hexString)
        {
            int length = hexString.Length / 2;

            if ((hexString.Length % 2) == 0)
            {
                for (int i = 0; i < length; i++)
                    WriteByte(Convert.ToByte(hexString.Substring(i * 2, 2), 16));
            }
            else
            {
                WriteByte(0);
            }
        }

        public virtual void WritePacketString(string packet)
        {
            packet = packet.Replace(" ", string.Empty);

            using (StringReader Reader = new StringReader(packet))
            {
                string Line;
                while ((Line = Reader.ReadLine()) != null)
                {
                    WriteHexStringBytes(Line.Substring(1, Line.IndexOf("|", 2)-1));
                }
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #region Mythic

        static public byte[] EncryptMythicRC4(byte[] Key,byte[] Packet)
        {
            try
            {
                int x, y, midpoint, pos;
                byte tmp = 0;

                x = y = 0;

                midpoint = Packet.Length / 2;

                for (pos = midpoint; pos < Packet.Length; ++pos)
                {
                    x = (x + 1) & 255;
                    y = (y + Key[x]) & 255;

                    tmp = Key[x];

                    Key[x] = Key[y];
                    Key[y] = tmp;

                    tmp = (byte)((Key[x] + Key[y]) & 255); 
                    y = (y + Packet[pos]) & 255;
                    Packet[pos] ^= Key[tmp];
                   
                }

                for (pos = 0; pos < midpoint; ++pos)
                {
                    x = (x + 1) & 255;
                    y = (y + Key[x]) & 255;

                    tmp = Key[x];

                    Key[x] = Key[y];
                    Key[y] = tmp;

                    tmp = (byte)((Key[x] + Key[y]) & 255);
                    y = (y + Packet[pos]) & 255;
                    Packet[pos] ^= Key[tmp];
                }

                return Packet;
            }
            catch (Exception e)
            {
                Log.Error("PacketOut", "EncryptMythicRC4 : Failled !"+e.ToString());
            }

            return null;

        }

        #endregion

        #region Gamebryo

        public void WriteGamebryoSize()
        {
            Position = 0;
            byte[] Total = ToArray();

            if (Total.Length - 1 < 0x80)
                WriteByte((byte)(Total.Length));
            else
            {
                int Size = Total.Length;
                int Offset = 1;

                while (Size >= (128*2))
                {
                    Size -= 128;
                    ++Offset;
                }

                WriteByte((byte)Size);
                WriteByte((byte)Offset);
            }

            Write(Total, 0, Total.Length);
        }

        #endregion
    }
}