using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "character_abilities", DatabaseName = "Characters")]
    [Serializable]
    public class Character_ability : DataObject
    {
        [DataElement()]
        public int CharacterID;

        [DataElement()]
        public ushort AbilityID;

        [DataElement()]
        public int LastCast;
    }
}
