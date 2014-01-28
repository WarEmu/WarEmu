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

namespace ByteOperations
{
    public class ByteSwap
    {
        public static UInt16 Swap(UInt16 input)
        {
            return ((UInt16)(
            ((0xFF00 & input) >> 8) |
            ((0x00FF & input) << 8)));
        }

        public static UInt32 Swap(UInt32 input)
        {
            return ((UInt32)(
            ((0xFF000000 & input) >> 24) |
            ((0x00FF0000 & input) >> 8) |
            ((0x0000FF00 & input) << 8) |
            ((0x000000FF & input) << 24)));
        }

        public static float Swap(float input)
        {
            byte[] tmpIn = BitConverter.GetBytes(input);
            byte[] tmpOut = new byte[4];
            tmpOut[0] = tmpIn[3];
            tmpOut[1] = tmpIn[2];
            tmpOut[2] = tmpIn[1];
            tmpOut[3] = tmpIn[0];
            return BitConverter.ToSingle(tmpOut, 0);
        }

        public static double Swap(double input)
        {
            byte[] tmpIn = BitConverter.GetBytes(input);
            byte[] tmpOut = new byte[8];
            tmpOut[0] = tmpIn[7];
            tmpOut[1] = tmpIn[6];
            tmpOut[2] = tmpIn[5];
            tmpOut[3] = tmpIn[4];
            tmpOut[4] = tmpIn[3];
            tmpOut[5] = tmpIn[2];
            tmpOut[6] = tmpIn[1];
            tmpOut[7] = tmpIn[0];
            return BitConverter.ToSingle(tmpOut, 0);
        }

        public static long Swap(long input)
        {
            byte[] tmpIn = BitConverter.GetBytes(input);
            byte[] tmpOut = new byte[BitConverter.GetBytes(input).Length];

            for (int i = 0; i < BitConverter.GetBytes(input).Length; i++)
                tmpOut[i] = tmpIn[(BitConverter.GetBytes(input).Length - 1) - i];

            return BitConverter.ToInt64(tmpOut, 0);
        }

        public static ulong Swap(ulong input)
        {
            byte[] tmpIn = BitConverter.GetBytes(input);
            byte[] tmpOut = new byte[BitConverter.GetBytes(input).Length];

            for (int i = 0; i < BitConverter.GetBytes(input).Length; i++)
                tmpOut[i] = tmpIn[(BitConverter.GetBytes(input).Length - 1) - i];

            return BitConverter.ToUInt64(tmpOut, 0);
        }

        public static void UInt32_To_BE(uint n, byte[] bs)
        {
            bs[0] = (byte)(n >> 24);
            bs[1] = (byte)(n >> 16);
            bs[2] = (byte)(n >> 8);
            bs[3] = (byte)(n);
        }

        public static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n >> 24);
            bs[++off] = (byte)(n >> 16);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n);
        }

        public static uint BE_To_UInt32(byte[] bs)
        {
            uint n = (uint)bs[0] << 24;
            n |= (uint)bs[1] << 16;
            n |= (uint)bs[2] << 8;
            n |= (uint)bs[3];
            return n;
        }

        public static uint BE_To_UInt32(byte[] bs, int off)
        {
            uint n = (uint)bs[off] << 24;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off];
            return n;
        }

        public static ulong BE_To_UInt64(byte[] bs)
        {
            uint hi = BE_To_UInt32(bs);
            uint lo = BE_To_UInt32(bs, 4);
            return ((ulong)hi << 32) | (ulong)lo;
        }

        public static ulong BE_To_UInt64(byte[] bs, int off)
        {
            uint hi = BE_To_UInt32(bs, off);
            uint lo = BE_To_UInt32(bs, off + 4);
            return ((ulong)hi << 32) | (ulong)lo;
        }

        public static void UInt64_To_BE(ulong n, byte[] bs)
        {
            UInt32_To_BE((uint)(n >> 32), bs);
            UInt32_To_BE((uint)(n), bs, 4);
        }

        public static void UInt64_To_BE(ulong n, byte[] bs, int off)
        {
            UInt32_To_BE((uint)(n >> 32), bs, off);
            UInt32_To_BE((uint)(n), bs, off + 4);
        }

        public static void UInt32_To_LE(uint n, byte[] bs)
        {
            bs[0] = (byte)(n);
            bs[1] = (byte)(n >> 8);
            bs[2] = (byte)(n >> 16);
            bs[3] = (byte)(n >> 24);
        }

        public static void UInt32_To_LE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n >> 16);
            bs[++off] = (byte)(n >> 24);
        }

        public static uint LE_To_UInt32(byte[] bs)
        {
            uint n = (uint)bs[0];
            n |= (uint)bs[1] << 8;
            n |= (uint)bs[2] << 16;
            n |= (uint)bs[3] << 24;
            return n;
        }

        public static uint LE_To_UInt32(byte[] bs, int off)
        {
            uint n = (uint)bs[off];
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 24;
            return n;
        }

        public static ulong LE_To_UInt64(byte[] bs)
        {
            uint lo = LE_To_UInt32(bs);
            uint hi = LE_To_UInt32(bs, 4);
            return ((ulong)hi << 32) | (ulong)lo;
        }

        public static ulong LE_To_UInt64(byte[] bs, int off)
        {
            uint lo = LE_To_UInt32(bs, off);
            uint hi = LE_To_UInt32(bs, off + 4);
            return ((ulong)hi << 32) | (ulong)lo;
        }

        public static void UInt64_To_LE(ulong n, byte[] bs)
        {
            UInt32_To_LE((uint)(n), bs);
            UInt32_To_LE((uint)(n >> 32), bs, 4);
        }

        public static void UInt64_To_LE(ulong n, byte[] bs, int off)
        {
            UInt32_To_LE((uint)(n), bs, off);
            UInt32_To_LE((uint)(n >> 32), bs, off + 4);
        }
    }
}
