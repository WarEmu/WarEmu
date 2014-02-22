using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "Characters_mails", DatabaseName = "Characters")]
    [Serializable]
    public class Character_mail : DataObject
    {

        private int _Guid;
        private int _CharacterId;
        private int _CharacterIdSender;
        private string _SenderName;
        private string _ReceiverName;
        private string _Title;
        private string _Content;
        private UInt32 _Money;
        private bool _Cr;
        private bool _Opened;
        private UInt16 _Items;

        [PrimaryKey(AutoIncrement = true)]
        public int Guid
        {
            get { return _Guid; }
            set { _Guid = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int CharacterIdSender
        {
            get { return _CharacterIdSender; }
            set { _CharacterIdSender = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string SenderName
        {
            get { return _SenderName; }
            set { _SenderName = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ReceiverName
        {
            get { return _ReceiverName; }
            set { _ReceiverName = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Content
        {
            get { return _Content; }
            set { _Content = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 Money
        {
            get { return _Money; }
            set { _Money = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public bool Cr
        {
            get { return _Cr; }
            set { _Cr = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public bool Opened
        {
            get { return _Opened; }
            set { _Opened = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Items
        {
            get { return _Items; }
            set { _Items = value; Dirty = true; }
        }

        //public Dictionary<Item_Info, uint> Items = new Dictionary<Item_Info, uint>();
    }
}
