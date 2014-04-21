/*
 * Copyright (C) 2013 APS
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

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "mount_infos", DatabaseName = "World")]
    [Serializable]
    public class Mount_Info : DataObject
    {
        [DataElement()]
        public uint Id;

        [DataElement(Unique=true,AllowDbNull=false)]
        public uint Entry;

        [DataElement(AllowDbNull=false)]
        public UInt16 Speed;

        [DataElement(AllowDbNull = false, Varchar = 255)]
        public string Name;
    }
}
