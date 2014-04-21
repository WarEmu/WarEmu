using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "zone_taxis", DatabaseName = "World")]
    [Serializable]
    public class Zone_Taxi : DataObject
    {
        [DataElement()]
        public UInt16 ZoneID;

        [DataElement()]
        public byte RealmID;

        [DataElement()]
        public UInt32 WorldX;

        [DataElement()]
        public UInt32 WorldY;

        [DataElement()]
        public UInt16 WorldZ;

        [DataElement()]
        public UInt16 WorldO;

        public Zone_Info Info;
    }
}
