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
using System.Text;
using System.Reflection;
using System.IO;

namespace FrameWork
{
    public class DatabaseInfo
    {
        public string Server = "127.0.0.1";
        public string Port = "3306";
        public string Database = "databasename";
        public string Username = "root";
        public string Password = "password";
        public string Custom = "Treat Tiny As Boolean=False";

        public string Total()
        {
            string Result = "";
            Result += "Server=" + Server + ";";
            Result += "Port=" + Port + ";";
            Result += "Database=" + Database + ";";
            Result += "User Id=" + Username + ";";
            Result += "Password=" + Password + ";";
            Result += Custom;
            return Result;
        }
    }
      

    public class DBManager
    {
        private readonly FileInfo _file = new FileInfo("sql.conf");

        public static MySQLObjectDatabase Start(string sqlconfig, ConnectionType Type, string DBName)
        {
            Log.Debug("IObjectDatabase", DBName + "->Start " + sqlconfig + "...");
            IObjectDatabase _database = null;

            try
            {
                _database = ObjectDatabase.GetObjectDatabase(Type, sqlconfig);
                if (_database == null)
                    return null;

                LoadTables(_database,DBName);

                return (MySQLObjectDatabase)_database;
            }
            catch
            {
                return null;
            }
        }

        static public void LoadTables(IObjectDatabase Database,string DatabaseName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass != true)
                        continue;

                    try
                    {
                        DataTable[] attrib = (DataTable[])type.GetCustomAttributes(typeof(DataTable), true);
                        if (attrib.Length > 0 && attrib[0].DatabaseName == DatabaseName)
                        {
                            Log.Info("DBManager", "Registering table: " + type.FullName);
                            Database.RegisterDataObject(type);
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Error("DBManager", "Can not load : " + e.ToString());
                    }
                }
            }
        }
    }
}
