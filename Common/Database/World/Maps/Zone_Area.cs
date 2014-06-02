/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
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
using System.Linq;
using System.Text;
using System.Drawing;

using FrameWork;

namespace Common
{

    [DataTable(PreCache = false, TableName = "zone_areas", DatabaseName = "World")]
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
        public uint OrderInfluenceId;

        [DataElement()]
        public uint DestroInfluenceId;

        [DataElement()]
        public ushort TokExploreEntry;

        public override string ToString()
        {
            return "[Area] " + AreaId + " PieceId=" + PieceId;
        }
    }
}
