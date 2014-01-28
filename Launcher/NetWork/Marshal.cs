
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher
{
    public static class Marshal
    {
        public static string ConvertToString(byte[] cstyle)
        {
            if (cstyle == null)
                return null;

            for (int i = 0; i < cstyle.Length; i++)
            {
                if (cstyle[i] == 0)
                    return UTF8Encoding.UTF8.GetString(cstyle, 0, i);
            }
            return UTF8Encoding.UTF8.GetString(cstyle);
        }

        public static int ConvertToInt32(byte[] val)
        {
            return ConvertToInt32(val, 0);
        }

        public static int ConvertToInt32(byte[] val, int startIndex)
        {
            return ConvertToInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
        }

        public static int ConvertToInt32(byte v1, byte v2, byte v3, byte v4)
        {
            return ((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
        }

        public static uint ConvertToUInt32(byte[] val)
        {
            return ConvertToUInt32(val, 0);
        }

        public static uint ConvertToUInt32(byte[] val, int startIndex)
        {
            return ConvertToUInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
        }

        public static uint ConvertToUInt32(byte v1, byte v2, byte v3, byte v4)
        {
            return (uint)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
        }

        public static float ConvertToFloat(byte v1, byte v2, byte v3, byte v4)
        {
            return (float)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
        }

        public static short ConvertToInt16(byte[] val)
        {
            return ConvertToInt16(val, 0);
        }

        public static short ConvertToInt16(byte[] val, int startIndex)
        {
            return ConvertToInt16(val[startIndex], val[startIndex + 1]);
        }

        public static short ConvertToInt16(byte v1, byte v2)
        {
            return (short)((v1 << 8) | v2);
        }

        public static ushort ConvertToUInt16(byte[] val)
        {
            return ConvertToUInt16(val, 0);
        }

        public static ushort ConvertToUInt16(byte[] val, int startIndex)
        {
            return ConvertToUInt16(val[startIndex], val[startIndex + 1]);
        }

        public static ushort ConvertToUInt16(byte v1, byte v2)
        {
            return (ushort)(v2 | (v1 << 8));
        }
    }
}
