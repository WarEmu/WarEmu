/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
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
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "auctions", DatabaseName = "Characters")]
    [Serializable]
    public class Auction : DataObject
    {
        private UInt64 _AuctionId;
        private byte _Realm;
        private UInt32 _SellerId;
        private UInt32 _ItemId;
        private UInt32 _SellPrice;
        private UInt32 _StartTime;

        public Auction()
            : base()
        {

        }

        [DataElement(Unique = true, AllowDbNull = false)]
        public UInt64 AuctionId
        {
            get { return _AuctionId; }
            set { _AuctionId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Realm
        {
            get { return _Realm; }
            set { _Realm = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false, Varchar = 255)]
        public UInt32 SellerId
        {
            get { return _SellerId; }
            set { _SellerId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 ItemId
        {
            get { return _ItemId; }
            set { _ItemId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 SellPrice
        {
            get { return _SellPrice; }
            set { _SellPrice = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; Dirty = true; }
        }

        public Item_Info Item;
        public Character Seller;
    }
}