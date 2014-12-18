﻿/*
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
using System.Text;

namespace FrameWork
{
    public interface IObjectDatabase
    {
        bool AddObject(DataObject dataObject);

        void SaveObject(DataObject dataObject);

        void DeleteObject(DataObject dataObject);

        TObject FindObjectByKey<TObject>(object key)
            where TObject : DataObject;

        TObject SelectObject<TObject>(string whereExpression)
            where TObject : DataObject;

        TObject SelectObject<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject;

        IList<TObject> SelectObjects<TObject>(string whereExpression)
            where TObject : DataObject;

        IList<TObject> SelectObjects<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject;

        IList<TObject> SelectAllObjects<TObject>()
            where TObject : DataObject;

        IList<TObject> SelectAllObjects<TObject>(IsolationLevel isolation)
            where TObject : DataObject;

        int GetNextAutoIncrement<TObject>()
            where TObject : DataObject;

        int GetObjectCount<TObject>()
            where TObject : DataObject;

        int GetObjectCount<TObject>(string whereExpression)
            where TObject : DataObject;

        void RegisterDataObject(Type dataObjectType);

        bool UpdateInCache<TObject>(object key)
            where TObject : DataObject;

        void FillObjectRelations(DataObject dataObject);

        string Escape(string rawInput);

        bool ExecuteNonQuery(string rawQuery);
    }
}
