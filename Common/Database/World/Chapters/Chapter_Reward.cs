using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "chapter_rewards", DatabaseName = "World")]
    [Serializable]
    public class Chapter_Reward : DataObject
    {
        [DataElement()]
        public UInt32 Entry;

        [DataElement()]
        public byte Realm;

        [DataElement()]
        public UInt32 ItemId;

        [DataElement()]
        public UInt32 InfluenceCount;

        public Chapter_Info Chapter;
        public Item_Info Item;
    }
}
