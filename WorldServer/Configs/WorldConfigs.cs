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

using FrameWork;

namespace WorldServer
{
    [aConfigAttributes("Configs/World.xml")]
    public class WorldConfigs : aConfig
    {
        public RpcClientConfig AccountCacherInfo = new RpcClientConfig("127.0.0.1", "127.0.0.1", 6800);
        public LogInfo LogLevel = new LogInfo();

        public DatabaseInfo CharacterDatabase = new DatabaseInfo();
        public DatabaseInfo WorldDatabase = new DatabaseInfo();

        public byte RealmId = 1;
        public int GlobalLootRate = 1;
        public int CommonLootRate = 1;
        public int UncommonLootRate = 1;
        public int RareLootRate = 1;
        public int VeryRareLootRate = 1;
        public int ArtifactLootRate = 1;
        public int GoldRate = 1;
        public int XpRate = 1;
        public int RenownRate = 1;
        public bool ChatBetweenRealms = true;
        public bool CreateBothRealms = true;
        public bool CleanSpawns = true;
        public bool DiscoverAll = false;
        public bool OpenRvR = false;

        public string ZoneFolder = "zones/";
    }
}
