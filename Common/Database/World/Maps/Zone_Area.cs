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
using System.Drawing;

using FrameWork;

namespace Common
{
    public class PieceInformation
    {
        public byte PieceId = 0;
        public UInt16 OffsetX = 0;
        public UInt16 OffsetY = 0;
        public UInt16 Width = 0;
        public UInt16 Height = 0;
        public Bitmap File = null;
        public bool Loaded = false;
    }

    [DataTable(PreCache = false, TableName = "Zone_Areas", DatabaseName = "World")]
    [Serializable]
    public class Zone_Area : DataObject
    {
        [DataElement()]
        public UInt16 ZoneId;

        [DataElement()]
        public UInt16 AreaId;

        [DataElement()]
        public byte Realm;

        [DataElement()]
        public byte PieceId;

        [DataElement()]
        public uint InfluenceId;

        [DataElement()]
        public uint TokExploreEntry;

        public PieceInformation Information;

        public bool IsOnArea(ushort PinX, ushort PinY)
        {
            if (Information == null)
                return false;

            PinX = (ushort)(PinX / 64);
            PinY = (ushort)(PinY / 64);

            //Log.Info(InfluenceId+",IsOnArea,"+PieceId, "PinX=" + PinX + ",PinY=" + PinY + ",OX=" + Information.OffsetX + ",OY=" + Information.OffsetY + ",Size=" + Information.Width + "," + Information.Height);

            if (Information.OffsetX > PinX)
                return false;

            if (Information.OffsetY > PinY)
                return false;

            if (Information.OffsetX + Information.Width < PinX)
                return false;

            if (Information.OffsetY + Information.Height < PinY)
                return false;

            return true;
        }

        public bool IsLoaded()
        {
            if (Information == null)
                return true;

            return Information.Loaded;
        }

        public override string ToString()
        {
            return "[Area] " + AreaId + " PieceId=" + PieceId;
        }
    }
}
