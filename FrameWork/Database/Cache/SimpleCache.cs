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
using System.Collections;

namespace FrameWork
{
    public class SimpleCache : ICache
    {
        private readonly Hashtable _cache = Hashtable.Synchronized(new Hashtable());

        #region ICache Members

        // Retourne toutes les clef enregitré dans la collection
        public ICollection Keys
        {
            get { return _cache.Keys; }
        }

        // Ajoute ou récupère un objet a partir de sa clef
        public object this[object key]
        {
            get
            {
                var wr = _cache[key] as WeakReference;
                if (wr == null || !wr.IsAlive)
                {
                    _cache.Remove(key);
                    return null;
                }

                return wr.Target;
            }
            set
            {
                if (value == null)
                {
                    _cache.Remove(key);
                }
                else
                {
                    _cache[key] = new WeakReference(value);
                }
            }
        }

        #endregion
    }
}