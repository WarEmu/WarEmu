using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "zone_respawns", DatabaseName = "World")]
    [Serializable]
    public class Zone_Respawn : DataObject
    {
        [PrimaryKey(AutoIncrement = true)]
        public int RespawnID;

        [DataElement(AllowDbNull = false)]
        public int ZoneID;

        [DataElement(AllowDbNull = false)]
        public byte Realm;

        [DataElement(AllowDbNull=false)]
        public UInt16 PinX;

        [DataElement(AllowDbNull = false)]
        public UInt16 PinY;

        [DataElement(AllowDbNull = false)]
        public UInt16 PinZ;

        [DataElement(AllowDbNull = false)]
        public UInt16 WorldO;
    }
}
