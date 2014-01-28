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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class Relation : Attribute
    {
        public Relation()
        {
            LocalField = null;
            RemoteField = null;
            AutoLoad = true;
            AutoDelete = false;
        }
        
        // Relation entre plusieurs champs de table différente
        public string LocalField { get; set; }

        // Supprime la relation
        public string RemoteField { get; set; }

        // Chargement automatique de la table
        public bool AutoLoad { get; set; }

        // Suppression de l'objet automatiquement lorsque l'objet est supprimé dans le core
        public bool AutoDelete { get; set; }
    }
}
