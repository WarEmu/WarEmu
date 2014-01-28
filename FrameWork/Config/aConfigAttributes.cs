/*
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

namespace FrameWork
{
    public abstract class aConfig
    {
        public bool IConfiguredTheFile=false;
    }
    
    public delegate void ConfigMethod(aConfigAttributes Attributes,aConfig Conf,bool FirstLoad);

    [AttributeUsage(AttributeTargets.Method)]
    public class aConfigMethod : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class aConfigAttributes : Attribute
    {
        protected string _FileName;

        public aConfigAttributes(string FileName)
        {
            _FileName = FileName;
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
        }
    }
}
