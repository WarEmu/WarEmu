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
    [DataTable(PreCache = false, TableName = "Ability_stats", DatabaseName = "World")]
    [Serializable]
    public class Ability_Stats : DataObject
    {
        [DataElement()]
        public UInt16 Entry;

        [DataElement()]
        public byte Level;

        [DataElement()]
        public string Description;

        [DataElement()]
        public uint[] Damages;

        [DataElement()]
        public uint[] Heals;

        [DataElement()]
        public uint[] Times;

        [DataElement()]
        public uint[] Percents;

        [DataElement()]
        public uint[] Types;

        [DataElement()]
        public uint[] Radius;

        [DataElement()]
        public bool RadiusAroundTarget = false;

        public Ability_Info Info;

        public uint GetDamage(int Id)
        {
            if (Damages == null || Id >= Damages.Length)
                return 0;

            return Damages[Id];
        }

        public uint GetHeal(int Id)
        {
            if (Heals == null || Id >= Heals.Length)
                return 0;

            return Heals[Id];
        }

        public uint GetTime(int Id)
        {
            if (Times == null || Id >= Times.Length)
                return 0;

            return Times[Id];
        }

        public uint GetPercent(int Id)
        {
            if (Percents == null || Id >= Percents.Length)
                return 0;

            return Percents[Id];
        }

        public uint GetType(int Id)
        {
            if (Types == null || Id >= Types.Length)
                return 0;

            return Types[Id];
        }

        public uint GetRadius(int Id)
        {
            if (Radius == null || Id >= Radius.Length)
                return 0;

            return (uint)((float)Radius[Id] * 0.3048f);
        }
    }
}
