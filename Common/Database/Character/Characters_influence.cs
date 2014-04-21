using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "character_influences", DatabaseName = "Characters")]
    [Serializable]
    public class Characters_influence : DataObject
    {
        [DataElement(AllowDbNull = false)]
        public int CharacterId;

        [DataElement(AllowDbNull = false)]
        public UInt16 InfluenceId;

        [DataElement(AllowDbNull = false)]
        public UInt32 InfluenceCount;
    }
}
