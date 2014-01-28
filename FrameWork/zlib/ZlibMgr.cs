/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
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
    static public class ZlibMgr
    {
        static public byte[] GetResult(byte[] Input, bool compress)
        {
            return compress ? Compress(Input,zlibConst.Z_BEST_COMPRESSION, zlibConst.Z_NO_FLUSH) : Decompress(Input);
        }

        static public byte[] Compress(byte[] Input,int Compression,int Flush)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut,Compression);
            ZStream.FlushMode = Flush;

            Process(ZStream,Input);

            return OutPut.ToArray();
        }

        static public byte[] Decompress(byte[] Input)
        {
            MemoryStream OutPut = new MemoryStream();
            ZOutputStream ZStream = new ZOutputStream(OutPut);
            Process(ZStream,Input);

            return OutPut.ToArray();
        }

        static private void Process(ZOutputStream ZStream,byte[] Input)
        {
            try
            {
                ZStream.Write(Input, 0, Input.Length);
                ZStream.Flush();
                ZStream.Close();
            }
            catch (Exception e)
            {
                Log.Error("Zlib", "Process Error : " + e.ToString());
            }
        }
    }
}
