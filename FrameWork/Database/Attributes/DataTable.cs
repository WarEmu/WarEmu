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

namespace FrameWork
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTable : Attribute
    {
        public DataTable()
        {
            TableName = null;
            PreCache = false;
        }

        // Défini le nom de la table a charger
        public string TableName { get; set; }

        // Pour l'affichage de la table
        public string ViewName { get; set; }

        // Nom de la DB a utiliser
        public string DatabaseName { get; set; }

        // True si la table doit être préchargée pour optimiser les performances
        public bool PreCache { get; set; }
    }
}
