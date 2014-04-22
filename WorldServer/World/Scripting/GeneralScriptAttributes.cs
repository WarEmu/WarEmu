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

namespace WorldServer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GeneralScriptAttribute : Attribute
    {
        public bool GlobalScript = false; // True if the script is general to all objects or false if the script me be generated for each object
        public string ScriptName = "";
        public uint CreatureEntry = 0;
        public uint GameObjectEntry = 0;

        public GeneralScriptAttribute(bool GlobalScript, string ScriptName, uint CreatureEntry = 0, uint GameObjectEntry = 0)
        {
            this.GlobalScript = GlobalScript;
            this.ScriptName = ScriptName;
            this.CreatureEntry = CreatureEntry;
            this.GameObjectEntry = GameObjectEntry;
        }
    }
}
