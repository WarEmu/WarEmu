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
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

using MySql.Data.MySqlClient;
using MySql.Data.Types;
using DataTable = System.Data.DataTable;

namespace FrameWork
{
    public abstract class ObjectDatabase : IObjectDatabase
    {
        protected static readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;
        private readonly Dictionary<Type, BindingInfo[]> BindingInfos = new Dictionary<Type, BindingInfo[]>();
        protected readonly DataConnection Connection;
        private readonly Dictionary<Type, ConstructorInfo> ConstructorByFieldType = new Dictionary<Type, ConstructorInfo>();
        private readonly Dictionary<Type, MemberInfo[]> MemberInfoCache = new Dictionary<Type, MemberInfo[]>();
        private readonly Dictionary<MemberInfo, Relation[]> RelationAttributes = new Dictionary<MemberInfo, Relation[]>();

        protected readonly Dictionary<string, DataTableHandler> TableDatasets;

        protected ObjectDatabase(DataConnection connection)
        {
            TableDatasets = new Dictionary<string, DataTableHandler>();
            Connection = connection;
        }

        #region Data tables

        protected DataSet GetDataSet(string tableName)
        {
            if (!TableDatasets.ContainsKey(tableName))
                return null;

            return TableDatasets[tableName].DataSet;
        }

        protected void FillObjectWithRow<TObject>(ref TObject dataObject, DataRow row, bool reload)
            where TObject : DataObject
        {
            bool relation = false;

            string tableName = dataObject.TableName;
            Type myType = dataObject.GetType();
            string id = row[tableName + "_ID"].ToString();

            MemberInfo[] myMembers = myType.GetMembers();
            dataObject.ObjectId = id;

            for (int i = 0; i < myMembers.Length; i++)
            {
                object[] myAttributes = GetRelationAttributes(myMembers[i]);

                if (myAttributes.Length > 0)
                {
                    //if(myAttributes[0] is Attributes.Relation)
                    //{
                    relation = true;
                    //}
                }
                else
                {
                    object[] keyAttrib = myMembers[i].GetCustomAttributes(typeof(PrimaryKey), true);
                    myAttributes = myMembers[i].GetCustomAttributes(typeof(DataElement), true);
                    if (myAttributes.Length > 0 || keyAttrib.Length > 0)
                    {
                        object val = row[myMembers[i].Name];
                        if (val != null && !val.GetType().IsInstanceOfType(DBNull.Value))
                        {
                            if (myMembers[i] is PropertyInfo)
                            {
                                ((PropertyInfo)myMembers[i]).SetValue(dataObject, val, null);
                            }
                            if (myMembers[i] is FieldInfo)
                            {
                                ((FieldInfo)myMembers[i]).SetValue(dataObject, val);
                            }
                        }
                    }
                }
            }

            dataObject.Dirty = false;


            if (relation)
            {
                FillLazyObjectRelations(dataObject, true);
            }

            dataObject.IsValid = true;
        }

        protected void FillRowWithObject(DataObject dataObject, DataRow row)
        {
            bool relation = false;

            Type myType = dataObject.GetType();

            row[dataObject.TableName + "_ID"] = dataObject.ObjectId;

            MemberInfo[] myMembers = myType.GetMembers();

            for (int i = 0; i < myMembers.Length; i++)
            {
                object[] myAttributes = GetRelationAttributes(myMembers[i]);
                object val = null;

                if (myAttributes.Length > 0)
                {
                    relation = true;
                }
                else
                {
                    myAttributes = myMembers[i].GetCustomAttributes(typeof(DataElement), true);
                    object[] keyAttrib = myMembers[i].GetCustomAttributes(typeof(PrimaryKey), true);

                    if (myAttributes.Length > 0 || keyAttrib.Length > 0)
                    {
                        if (myMembers[i] is PropertyInfo)
                        {
                            val = ((PropertyInfo)myMembers[i]).GetValue(dataObject, null);
                        }
                        if (myMembers[i] is FieldInfo)
                        {
                            val = ((FieldInfo)myMembers[i]).GetValue(dataObject);
                        }
                        if (val != null)
                        {
                            row[myMembers[i].Name] = val;
                        }
                    }
                }
                //}
            }

            if (relation)
            {
                SaveObjectRelations(dataObject);
            }
        }

        protected DataRow FindRowByKey(DataObject dataObject)
        {
            DataRow row;

            string tableName = dataObject.TableName;


            System.Data.DataTable table = GetDataSet(tableName).Tables[tableName];

            Type myType = dataObject.GetType();

            string key = table.PrimaryKey[0].ColumnName;

            if (key.Equals(tableName + "_ID"))
                row = table.Rows.Find(dataObject.ObjectId);
            else
            {
                MemberInfo[] keymember = myType.GetMember(key);

                object val = null;

                if (keymember[0] is PropertyInfo)
                    val = ((PropertyInfo)keymember[0]).GetValue(dataObject, null);
                if (keymember[0] is FieldInfo)
                    val = ((FieldInfo)keymember[0]).GetValue(dataObject);

                if (val != null)
                    row = table.Rows.Find(val);
                else
                    return null;
            }

            return row;
        }

        #endregion

        #region Public API

        // Ajoute un nouvel objet dans la DB et le sauvegarde
        public bool AddObject(DataObject dataObject)
        {
            if (dataObject.AllowAdd)
            {
                return AddObjectImpl(dataObject);
            }
            else
            {
                Log.Notice("ObjectDatabase","Can not save, AllowSave = False " + dataObject.TableName + " : " + dataObject.ObjectId);
                return false;
            }
        }

        // Sauvegarde l'objet dans la db
        public void SaveObject(DataObject dataObject)
        {
            if (dataObject.Dirty)
            {
                SaveObjectImpl(dataObject);
            }
        }

        // Supprime l'objet de la DB
        public void DeleteObject(DataObject dataObject)
        {
            if (dataObject.AllowDelete)
            {
                DeleteObjectImpl(dataObject);
            }
        }

        public int GetObjectCount<TObject>()
            where TObject : DataObject
        {
            return GetObjectCount<TObject>("");
        }

        public int GetObjectCount<TObject>(string whereExpression)
            where TObject : DataObject
        {
            return GetObjectCountImpl<TObject>(whereExpression);
        }

        public TObject FindObjectByKey<TObject>(object key)
            where TObject : DataObject
        {
            var dataObject = FindObjectByKeyImpl<TObject>(key);

            return dataObject ?? default(TObject);
        }


        // Sélectionne un objet , si il y en a plusieurs , le premier et retourné
        public TObject SelectObject<TObject>(string whereExpression)
            where TObject : DataObject
        {
            return SelectObject<TObject>(whereExpression, IsolationLevel.DEFAULT);
        }

        // Sélectionne un objet , si il y en a plusieurs , le premier et retourné
        public TObject SelectObject<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject
        {
            var objs = SelectObjects<TObject>(whereExpression, isolation);

            if (objs.Count > 0)
                return objs[0];

            return default(TObject);
        }

        public IList<TObject> SelectObjects<TObject>(string whereExpression)
            where TObject : DataObject
        {
            return SelectObjects<TObject>(whereExpression, IsolationLevel.DEFAULT);
        }

        public IList<TObject> SelectObjects<TObject>(string whereExpression, IsolationLevel isolation)
            where TObject : DataObject
        {
            var dataObjects = SelectObjectsImpl<TObject>(whereExpression, isolation);

            return dataObjects ?? new List<TObject>();
        }

        public IList<TObject> SelectAllObjects<TObject>()
            where TObject : DataObject
        {
            return SelectAllObjects<TObject>(IsolationLevel.DEFAULT);
        }

        public IList<TObject> SelectAllObjects<TObject>(IsolationLevel isolation)
            where TObject : DataObject
        {
            var dataObjects = SelectAllObjectsImpl<TObject>(isolation);

            return dataObjects ?? new List<TObject>();
        }

        public void RegisterDataObject(Type objType)
        {
            if (TableDatasets.ContainsKey(GetTableOrViewName(objType)))
                return;

            bool primary = false;
            bool relations = false;
            MemberInfo primaryIndexMember = null;

            string tableName = GetTableOrViewName(objType);
            var ds = new DataSet();
            var table = new System.Data.DataTable(tableName);
            table.Columns.Add(tableName + "_ID", typeof(string));

            MemberInfo[] myMembers = objType.GetMembers();

            for (int i = 0; i < myMembers.Length; i++)
            {
                object[] myAttributes = myMembers[i].GetCustomAttributes(typeof(PrimaryKey), true);

                if (myAttributes.Length > 0)
                {
                    primary = true;
                    if (myMembers[i] is PropertyInfo)
                        table.Columns.Add(myMembers[i].Name, ((PropertyInfo)myMembers[i]).PropertyType);
                    else
                        table.Columns.Add(myMembers[i].Name, ((FieldInfo)myMembers[i]).FieldType);

                    table.Columns[myMembers[i].Name].AutoIncrement = ((PrimaryKey)myAttributes[0]).AutoIncrement;
                    table.Columns[myMembers[i].Name].AutoIncrementSeed = ((PrimaryKey)myAttributes[0]).IncrementValue;

                    var index = new DataColumn[1];
                    index[0] = table.Columns[myMembers[i].Name];
                    primaryIndexMember = myMembers[i];
                    table.PrimaryKey = index;
                    continue;
                }

                myAttributes = myMembers[i].GetCustomAttributes(typeof(DataElement), true);

                if (myAttributes.Length > 0)
                {
                    if (myMembers[i] is PropertyInfo)
                    {
                        table.Columns.Add(myMembers[i].Name, ((PropertyInfo)myMembers[i]).PropertyType);
                    }
                    else
                    {
                        table.Columns.Add(myMembers[i].Name, ((FieldInfo)myMembers[i]).FieldType);
                    }

                    table.Columns[myMembers[i].Name].AllowDBNull = ((DataElement)myAttributes[0]).AllowDbNull;
                    if (((DataElement)myAttributes[0]).Unique)
                    {
                        table.Constraints.Add(new UniqueConstraint("UNIQUE_" + myMembers[i].Name, table.Columns[myMembers[i].Name]));
                    }
                    if (((DataElement)myAttributes[0]).Index)
                    {
                        table.Columns[myMembers[i].Name].ExtendedProperties.Add("INDEX", true);
                    }
                    if (((DataElement)myAttributes[0]).Varchar > 0)
                    {
                        table.Columns[myMembers[i].Name].ExtendedProperties.Add("VARCHAR", ((DataElement)myAttributes[0]).Varchar);
                    }

                    myAttributes = GetRelationAttributes(myMembers[i]);

                    if (myAttributes.Length > 0)
                    {
                        relations = true;
                    }
                }
            }

            if (primary == false)
            {
                var index = new DataColumn[1];
                index[0] = table.Columns[tableName + "_ID"];
                table.PrimaryKey = index;
            }

            if (Connection.IsSQLConnection)
            {
                Connection.CheckOrCreateTable(table);
            }

            ds.DataSetName = tableName;
            ds.EnforceConstraints = true;
            ds.CaseSensitive = false;
            ds.Tables.Add(table);

            var dth = new DataTableHandler(ds);
            dth.HasRelations = relations;
            dth.UsesPreCaching = DataObject.GetPreCachedFlag(objType);

            TableDatasets.Add(tableName, dth);
        }

        public string[] GetTableNameList()
        {
            var list = new List<string>();
            foreach (var entry in TableDatasets)
            {
                list.Add(entry.Key);
            }

            return list.ToArray();
        }

        public string Escape(string toEscape)
        {
            return Connection.Escape(toEscape);
        }

        public bool ExecuteNonQuery(string rawQuery)
        {
            return ExecuteNonQueryImpl(rawQuery);
        }

        #endregion

        #region Implementation

        // Ajoute un objet a la database , true = Success
        protected abstract bool AddObjectImpl(DataObject dataObject);

        // Sauvegarde un Objet dans la Database
        protected abstract void SaveObjectImpl(DataObject dataObject);

        // Supprime un objet de la database
        protected abstract void DeleteObjectImpl(DataObject dataObject);

        // Trouve un objet a partir de sa primaryKey
        protected abstract TObject FindObjectByKeyImpl<TObject>(object key)
            where TObject : DataObject;

        // Trouve un objet a partir de sa primaryKey
        protected abstract DataObject FindObjectByKeyImpl(Type objectType, object key);

        // Sélectionne un objet a partir d'une table et des paramètres
        protected abstract DataObject[] SelectObjectsImpl(Type objectType, string whereClause, IsolationLevel isolation);

        // Sélectionne un objet a partir d'une table et des paramètres
        protected abstract IList<TObject> SelectObjectsImpl<TObject>(string whereClause, IsolationLevel isolation)
            where TObject : DataObject;

        // Sélectionne tous les objets de la database
        protected abstract IList<TObject> SelectAllObjectsImpl<TObject>(IsolationLevel isolation)
            where TObject : DataObject;

        // Retourn le nombre d'objet dans la database
        protected abstract int GetObjectCountImpl<TObject>(string where)
            where TObject : DataObject;

        protected abstract bool ExecuteNonQueryImpl(string raqQuery);

        #endregion

        #region Relations

        public void FillObjectRelations(DataObject dataObject)
        {
            FillLazyObjectRelations(dataObject, false);
        }

        protected void SaveObjectRelations(DataObject dataObject)
        {
            try
            {
                object val;

                Type myType = dataObject.GetType();

                MemberInfo[] myMembers = myType.GetMembers();

                for (int i = 0; i < myMembers.Length; i++)
                {
                    Relation[] myAttributes = GetRelationAttributes(myMembers[i]);
                    if (myAttributes.Length > 0)
                    {
                        bool array = false;

                        Type type;

                        if (myMembers[i] is PropertyInfo)
                            type = ((PropertyInfo)myMembers[i]).PropertyType;
                        else
                            type = ((FieldInfo)myMembers[i]).FieldType;

                        if (type.HasElementType)
                        {
                            type = type.GetElementType();
                            array = true;
                        }

                        val = null;

                        if (array)
                        {
                            if (myMembers[i] is PropertyInfo)
                            {
                                val = ((PropertyInfo)myMembers[i]).GetValue(dataObject, null);
                            }
                            if (myMembers[i] is FieldInfo)
                            {
                                val = ((FieldInfo)myMembers[i]).GetValue(dataObject);
                            }
                            if (val is Array)
                            {
                                var a = val as Array;

                                foreach (object o in a)
                                {
                                    if (o is DataObject)
                                        SaveObject(o as DataObject);
                                }
                            }
                            else
                            {
                                if (val is DataObject)
                                    SaveObject(val as DataObject);
                            }
                        }
                        else
                        {
                            if (myMembers[i] is PropertyInfo)
                                val = ((PropertyInfo)myMembers[i]).GetValue(dataObject, null);
                            if (myMembers[i] is FieldInfo)
                                val = ((FieldInfo)myMembers[i]).GetValue(dataObject);
                            if (val != null && val is DataObject)
                                SaveObject(val as DataObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DatabaseException("Relation save failed !", e);
            }
        }

        protected void DeleteObjectRelations(DataObject dataObject)
        {
            try
            {
                object val;

                Type myType = dataObject.GetType();

                MemberInfo[] myMembers = myType.GetMembers();

                for (int i = 0; i < myMembers.Length; i++)
                {
                    Relation[] myAttributes = GetRelationAttributes(myMembers[i]);
                    if (myAttributes.Length > 0)
                    {
                        if (myAttributes[0].AutoDelete == false)
                            continue;

                        bool array = false;

                        Type type;

                        if (myMembers[i] is PropertyInfo)
                            type = ((PropertyInfo)myMembers[i]).PropertyType;
                        else
                            type = ((FieldInfo)myMembers[i]).FieldType;

                        if (type.HasElementType)
                        {
                            type = type.GetElementType();
                            array = true;
                        }

                        val = null;

                        if (array)
                        {
                            if (myMembers[i] is PropertyInfo)
                            {
                                val = ((PropertyInfo)myMembers[i]).GetValue(dataObject, null);
                            }
                            if (myMembers[i] is FieldInfo)
                            {
                                val = ((FieldInfo)myMembers[i]).GetValue(dataObject);
                            }
                            if (val is Array)
                            {
                                var a = val as Array;

                                foreach (object o in a)
                                {
                                    if (o is DataObject)
                                        DeleteObject(o as DataObject);
                                }
                            }
                            else
                            {
                                if (val is DataObject)
                                    DeleteObject(val as DataObject);
                            }
                        }
                        else
                        {
                            if (myMembers[i] is PropertyInfo)
                                val = ((PropertyInfo)myMembers[i]).GetValue(dataObject, null);
                            if (myMembers[i] is FieldInfo)
                                val = ((FieldInfo)myMembers[i]).GetValue(dataObject);
                            if (val != null && val is DataObject)
                                DeleteObject(val as DataObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DatabaseException("Relations delete failed !", e);
            }
        }

        protected void FillLazyObjectRelations(DataObject dataObject, bool autoload)
        {
            try
            {
                var dataObjectType = dataObject.GetType();

                MemberInfo[] myMembers;
                if (!MemberInfoCache.TryGetValue(dataObjectType, out myMembers))
                {
                    myMembers = dataObjectType.GetMembers();
                    MemberInfoCache[dataObjectType] = myMembers;
                }

                for (int i = 0; i < myMembers.Length; i++)
                {
                    Relation[] myAttributes = GetRelationAttributes(myMembers[i]);

                    if (myAttributes.Length > 0)
                    {
                        Relation rel = myAttributes[0];

                        if ((rel.AutoLoad == false) && autoload)
                            continue;

                        bool isArray = false;
                        Type remoteType;
                        DataObject[] elements;

                        string local = rel.LocalField;
                        string remote = rel.RemoteField;

                        if (myMembers[i] is PropertyInfo)
                        {
                            remoteType = ((PropertyInfo)myMembers[i]).PropertyType;
                        }
                        else
                        {
                            remoteType = ((FieldInfo)myMembers[i]).FieldType;
                        }

                        if (remoteType.HasElementType)
                        {
                            remoteType = remoteType.GetElementType();
                            isArray = true;
                        }

                        PropertyInfo prop = dataObjectType.GetProperty(local);
                        FieldInfo field = dataObjectType.GetField(local);

                        object val = 0;

                        if (prop != null)
                        {
                            val = prop.GetValue(dataObject, null);
                        }
                        if (field != null)
                        {
                            val = field.GetValue(dataObject);
                        }

                        if (val != null && val.ToString() != string.Empty)
                        {
                            if (DataObject.GetPreCachedFlag(remoteType))
                            {
                                elements = new DataObject[1];
                                elements[0] = FindObjectByKeyImpl(remoteType, val);
                            }
                            else
                            {
                                elements = SelectObjectsImpl(remoteType, remote + " = '" + Escape(val.ToString()) + "'", IsolationLevel.DEFAULT);
                            }

                            if ((elements != null) && (elements.Length > 0))
                            {
                                if (isArray)
                                {
                                    if (myMembers[i] is PropertyInfo)
                                    {
                                        ((PropertyInfo)myMembers[i]).SetValue(dataObject, elements, null);
                                    }
                                    if (myMembers[i] is FieldInfo)
                                    {
                                        var currentField = (FieldInfo)myMembers[i];
                                        ConstructorInfo constructor;
                                        if (!ConstructorByFieldType.TryGetValue(currentField.FieldType, out constructor))
                                        {
                                            constructor = currentField.FieldType.GetConstructor(new[] { typeof(int) });
                                            ConstructorByFieldType[currentField.FieldType] = constructor;
                                        }

                                        object elementHolder = constructor.Invoke(new object[] { elements.Length });
                                        var elementArray = (object[])elementHolder;

                                        for (int m = 0; m < elementArray.Length; m++)
                                        {
                                            elementArray[m] = elements[m];
                                        }

                                        currentField.SetValue(dataObject, elementArray);
                                    }
                                }
                                else
                                {
                                    if (myMembers[i] is PropertyInfo)
                                    {
                                        ((PropertyInfo)myMembers[i]).SetValue(dataObject, elements[0], null);
                                    }
                                    if (myMembers[i] is FieldInfo)
                                    {
                                        ((FieldInfo)myMembers[i]).SetValue(dataObject, elements[0]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DatabaseException("Resolution of relations " + dataObject.TableName + " failed!", e);
            }
        }

        #endregion

        #region Cache

        protected void DeleteFromCache(string tableName, DataObject obj)
        {
            DataTableHandler handler = TableDatasets[tableName];
            handler.SetCacheObject(obj.ObjectId, null);
        }

        // Met a jour ou ajoute un objet dans la DB
        public bool UpdateInCache<TObject>(object key)
            where TObject : DataObject
        {
            MemberInfo[] members = typeof(TObject).GetMembers();
            var ret = (TObject)Activator.CreateInstance(typeof(TObject));

            string tableName = ret.TableName;
            DataTableHandler dth = TableDatasets[tableName];
            string whereClause = null;

            if (!dth.UsesPreCaching || key == null)
                return false;

            // Escape PK value
            key = Escape(key.ToString());

            for (int i = 0; i < members.Length; i++)
            {
                object[] keyAttrib = members[i].GetCustomAttributes(typeof(PrimaryKey), true);
                if (keyAttrib.Length > 0)
                {
                    whereClause = "`" + members[i].Name + "` = '" + key + "'";
                    break;
                }
            }

            if (whereClause == null)
            {
                whereClause = "`" + ret.TableName + "_ID` = '" + key + "'";
            }

            var objs = SelectObjects<TObject>(whereClause);
            if (objs.Count > 0)
            {
                dth.SetPreCachedObject(key, objs[0]);
                return true;
            }

            return false;
        }

        protected void ReloadCache(string tableName)
        {
            DataTableHandler handler = TableDatasets[tableName];

            ICache cache = handler.Cache;

            foreach (object o in cache.Keys)
            {
                ReloadObject(cache[o] as DataObject);
            }
        }

        #endregion

        #region Helpers

        protected Relation[] GetRelationAttributes(MemberInfo info)
        {
            Relation[] rel;
            if (RelationAttributes.TryGetValue(info, out rel))
                return rel;

            rel = (Relation[])info.GetCustomAttributes(typeof(Relation), true);
            RelationAttributes[info] = rel;

            return rel;
        }

        protected BindingInfo[] GetBindingInfo(Type objectType)
        {
            BindingInfo[] bindingInfos;

            if (!BindingInfos.TryGetValue(objectType, out bindingInfos))
            {
                var list = new List<BindingInfo>();

                MemberInfo[] objMembers = objectType.GetMembers();
                for (int i = 0; i < objMembers.Length; i++)
                {
                    object[] keyAttrib = objMembers[i].GetCustomAttributes(typeof(PrimaryKey), true);
                    object[] readonlyAttrib = objMembers[i].GetCustomAttributes(typeof(ReadOnly), true);
                    object[] attrib = objMembers[i].GetCustomAttributes(typeof(DataElement), true);
                    object[] relAttrib = GetRelationAttributes(objMembers[i]);

                    if (attrib.Length > 0 || keyAttrib.Length > 0 || relAttrib.Length > 0 || readonlyAttrib.Length > 0)
                    {
                        var info = new BindingInfo(objMembers[i], keyAttrib.Length > 0, relAttrib.Length > 0, readonlyAttrib.Length > 0,
                                                   (attrib.Length > 0) ? (DataElement)attrib[0] : null);
                        list.Add(info);
                    }
                }

                bindingInfos = list.ToArray();
                BindingInfos[objectType] = bindingInfos;
            }

            return bindingInfos;
        }

        // Lecture de la clef primaire
        public static string GetTableOrViewName(Type objectType)
        {
            string name = DataObject.GetViewName(objectType);

            // if not a view, we use tablename, else viewname
            if (string.IsNullOrEmpty(name))
                return DataObject.GetTableName(objectType);

            return name;
        }

        private DataObject ReloadObject(DataObject dataObject)
        {
            try
            {
                if (dataObject == null)
                    return null;

                DataObject ret = dataObject;

                DataRow row = FindRowByKey(ret);

                if (row == null)
                    throw new DatabaseException("Reload of Databaseobject failed (Keyvalue Changed ?)!");

                FillObjectWithRow(ref ret, row, true);

                dataObject.Dirty = false;
                dataObject.IsValid = true;

                return ret;
            }
            catch (Exception e)
            {
                throw new DatabaseException("Reload of Databaseobject failed !", e);
            }
        }

        #endregion

        #region Factory

        public static IObjectDatabase GetObjectDatabase(ConnectionType connectionType, string connectionString)
        {
            var connection = new DataConnection(connectionType, connectionString);

            if (connectionType == ConnectionType.DATABASE_MYSQL)
                return new MySQLObjectDatabase(connection);

            return null;
        }

        #endregion

        #region Nested type: BindingInfo

        protected class BindingInfo
        {
            public readonly bool HasRelation;
            public readonly MemberInfo Member;
            public readonly bool ReadOnly;
            public DataElement DataElementAttribute;
            public bool PrimaryKey;

            public BindingInfo(MemberInfo member, bool primaryKey, bool hasRelation, bool readOnly, DataElement attrib)
            {
                Member = member;
                PrimaryKey = primaryKey;
                HasRelation = hasRelation;
                DataElementAttribute = attrib;
                ReadOnly = readOnly;
            }
        }

        #endregion
    }
}