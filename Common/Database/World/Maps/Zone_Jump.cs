using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "zone_jumps", DatabaseName = "World")]
    [Serializable]
    public class Zone_jump : DataObject
    {
        [DataElement(Unique = true)]
        public uint Entry;

        [DataElement(AllowDbNull = false)]
        public UInt16 ZoneID;

        [DataElement(AllowDbNull = false)]
        public uint WorldX;

        [DataElement(AllowDbNull = false)]
        public uint WorldY;

        [DataElement(AllowDbNull = false)]
        public ushort WorldZ;

        [DataElement(AllowDbNull = false)]
        public ushort WorldO;

        public Zone_Info ZoneInfo;
    }
}
