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
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

using MySql.Data.MySqlClient;

namespace FrameWork
{
    // Appelé après une requète Mysql
    public delegate void QueryCallback(MySqlDataReader reader);

    // Toutes les fonctions de chargement et de requètes
    public class DataConnection
    {
        // Liste des connexions Mysql
        private readonly Queue<MySqlConnection> m_connectionPool = new Queue<MySqlConnection>();

        private string connString;
        private ConnectionType connType;

        // Construction d'une connexion , Type(Mysql,ODBC,etc..) + paramètre de la connexion
        public DataConnection(ConnectionType connType, string connString)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            this.connType = connType;

            // Options de connexion pour Mysql
            if (!connString.Contains("Treat Tiny As Boolean"))
            {
                connString += ";Treat Tiny As Boolean=False";
            }

            this.connString = connString;
        }

        // Check si c'est une connexion Mysql
        public bool IsSQLConnection
        {
            get { return connType == ConnectionType.DATABASE_MYSQL; }
        }

        // Renvoi le type de la connexion
        public ConnectionType ConnectionType
        {
            get { return connType; }
        }

        // Supprimes les caractères non autorisé
        static public string SQLEscape(string s)
        {
            s = s.Replace("\\", "\\\\");
            s = s.Replace("\"", "\\\"");
            s = s.Replace("'", "\\'");
            s = s.Replace("’", "\\’");

            return s;
        }

        // Supprimes les caractères non autorisé
        public string Escape(string s)
        {
            if (!IsSQLConnection)
            {
                s = s.Replace("'", "''");
            }
            else
            {
                s = s.Replace("\\", "\\\\");
                s = s.Replace("\"", "\\\"");
                s = s.Replace("'", "\\'");
                s = s.Replace("’", "\\’");
            }
            return s;
        }

        // Renvoi une connexion mysql du pool
        private MySqlConnection GetMySqlConnection(out bool isNewConnection)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            // Get connection from pool
            MySqlConnection conn = null;
            lock (m_connectionPool)
            {
                if (m_connectionPool.Count > 0)
                {
                    conn = m_connectionPool.Dequeue();
                }
            }

            if (conn != null)
            {
                isNewConnection = false;
            }
            else
            {
                isNewConnection = true;
                long start1 = Environment.TickCount;
                conn = new MySqlConnection(connString);
                conn.Open();
                if (Environment.TickCount - start1 > 1000)
                {
                     Log.Notice("DataConnecion","Connection time : " + (Environment.TickCount - start1) + "ms");
                }

                Log.Debug("DataConnection", "New DB Connection");
            }

            return conn;
        }


        // Supprime la connexion Mysql du pool
        private void ReleaseConnection(MySqlConnection conn)
        {
            lock (m_connectionPool)
            {
                m_connectionPool.Enqueue(conn);
            }
        }

        // Exécute une requète non bloquante (insert,delete,update)
        public int ExecuteNonQuery(string sqlcommand)
        {
            if (connType == ConnectionType.DATABASE_MYSQL)
            {
               Log.Debug("DataConnection","SQL: " + sqlcommand);

                int affected = 0;
                bool repeat = false;
                do
                {
                    bool isNewConnection;
                    MySqlConnection conn = GetMySqlConnection(out isNewConnection);
                    var cmd = new MySqlCommand(sqlcommand, conn);

                    try
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        long start = Environment.TickCount;
                        affected = cmd.ExecuteNonQuery();

                        Log.Debug("DataConnection", "SQL NonQuery exec time " + (Environment.TickCount - start) + "ms");
                       
                        if (Environment.TickCount - start > 500)
                           Log.Notice("DataConnection", "SQL NonQuery took " + (Environment.TickCount - start) + "ms!\n" + sqlcommand);

                        ReleaseConnection(conn);

                        repeat = false;
                    }
                    catch (Exception e)
                    {
                        conn.Close();

                        if (!HandleException(e) || isNewConnection)
                        {
                            throw;
                        }
                        repeat = true;
                    }
                } while (repeat);

                return affected;
            }

            Log.Notice("DataConnection", "SQL NonQuery's not supported.");

            return 0;
        }

        // Gère les exeptions levé par la DB
        private static bool HandleException(Exception e)
        {
            bool ret = false;
            SocketException socketException = e.InnerException == null
                                                ? null
                                                : e.InnerException.InnerException as SocketException;
            if (socketException == null)
            {
                socketException = e.InnerException as SocketException;
            }

            if (socketException != null)
            {
                // Handle socket exception. Error codes:
                // http://msdn2.microsoft.com/en-us/library/ms740668.aspx
                // 10052 = Network dropped connection on reset.
                // 10053 = Software caused connection abort.
                // 10054 = Connection reset by peer.
                // 10057 = Socket is not connected.
                // 10058 = Cannot send after socket shutdown.
                switch (socketException.ErrorCode)
                {
                    case 10052:
                    case 10053:
                    case 10054:
                    case 10057:
                    case 10058:
                        {
                            ret = true;
                            break;
                        }
                }

                Log.Notice("DataConnection",string.Format("Socket exception: ({0}) {1}; repeat: {2}", socketException.ErrorCode, socketException.Message, ret));

            }

            return ret;
        }

        // Exécute un Select (bloquand) et retourn le DataSet correspondant
        public void ExecuteSelect(string sqlcommand, QueryCallback callback, IsolationLevel isolation)
        {
            if (connType == ConnectionType.DATABASE_MYSQL)
            {
               Log.Debug("DataConnecion","SQL: " + sqlcommand);

                bool repeat = false;
                MySqlConnection conn = null;
                do
                {
                    bool isNewConnection = true;
                    try
                    {
                        conn = GetMySqlConnection(out isNewConnection);

                        long start = Environment.TickCount;

                        var cmd = new MySqlCommand(sqlcommand, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        callback(reader);
                        reader.Close();

                        Log.Debug("DataConnecion","SQL Select (" + isolation + ") exec time " + (Environment.TickCount - start) + "ms");

                        if (Environment.TickCount - start > 500)
                            Log.Notice("DataConnecion", "SQL Select (" + isolation + ") took " + (Environment.TickCount - start) + "ms!\n" + sqlcommand);

                        ReleaseConnection(conn);

                        repeat = false;
                    }
                    catch (Exception e)
                    {
                        if (conn != null)
                        {
                            conn.Close();
                        }

                        if (!HandleException(e) || isNewConnection)
                        {
                            Log.Error("DataConnecion", "ExecuteSelect: \"" + sqlcommand + "\"\n" + e.ToString() );
                            return;
                        }

                        repeat = true;
                    }
                } while (repeat);

                return;
            }

            Log.Notice("DataConnecion", "SQL Selects not supported");
        }

        // Exécute un scale sur la DB
        public object ExecuteScalar(string sqlcommand)
        {
            if (connType == ConnectionType.DATABASE_MYSQL)
            {
                Log.Debug("DataConnecion","SQL: " + sqlcommand);

                object obj = null;
                bool repeat = false;
                MySqlConnection conn = null;
                do
                {
                    bool isNewConnection = true;
                    try
                    {
                        conn = GetMySqlConnection(out isNewConnection);
                        var cmd = new MySqlCommand(sqlcommand, conn);

                        long start = Environment.TickCount;
                        obj = cmd.ExecuteScalar();

                        ReleaseConnection(conn);

                        Log.Debug("DataConnecion","SQL Select exec time " + (Environment.TickCount - start) + "ms");
                        
                        if (Environment.TickCount - start > 500)
                            Log.Notice("DataConnecion", "SQL Select took " + (Environment.TickCount - start) + "ms!\n" + sqlcommand);

                        repeat = false;
                    }
                    catch (Exception e)
                    {
                        if (conn != null)
                        {
                            conn.Close();
                        }

                        if (!HandleException(e) || isNewConnection)
                        {
                            Log.Error("DataConnecion", "ExecuteSelect: \"" + sqlcommand + "\"\n" + e.ToString() );
                            throw;
                        }

                        repeat = true;
                    }
                } while (repeat);

                return obj;
            }

            Log.Notice("DataConnecion", "SQL Scalar not supported");

            return null;
        }

        // Vérifi ou créer la table a partir d'une DataTable
        public void CheckOrCreateTable(System.Data.DataTable table)
        {
            List<string> alterRemoveColumnDefs = new List<string>();
            List<string> columnDefs = new List<string>();
            List<string> alterAddColumnDefs = new List<string>();
            if (connType == ConnectionType.DATABASE_MYSQL)
            {
                ArrayList currentTableColumns = new ArrayList();
                try
                {
                    ExecuteSelect("DESCRIBE `" + table.TableName + "`", delegate(MySqlDataReader reader)
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        while (reader.Read())
                        {
                            currentTableColumns.Add(reader.GetString(0).ToLower());
                            if (reader.GetString(0).ToLower() != (table.TableName + "_ID").ToLower() && !table.Columns.Contains(reader.GetString(0).ToLower()))
                                alterRemoveColumnDefs.Add(reader.GetString(0).ToLower());

                            Log.Debug("DataConnecion", reader.GetString(0).ToLower());
                        }

                        Log.Debug("DataConnecion", currentTableColumns.Count + " in table");

                    }, IsolationLevel.DEFAULT);

                    if (!currentTableColumns.Contains((table.TableName + "_ID").ToLower()))
                    {
                        Log.Success("WAZA", "Creating Alter Primary Key");
                        ExecuteNonQuery("ALTER TABLE `" + table.TableName + "` ADD `"+table.TableName+"_ID`  VARCHAR( 255 ) NOT NULL;");
                        ExecuteNonQuery("ALTER TABLE `" + table.TableName + "` ADD PRIMARY KEY (`" + table.TableName + "_ID`);");
                        ExecuteNonQuery("UPDATE TABLE `" + table.TableName + "` SET " + table.TableName + "_ID=UUID();");
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("DataConnecion", e.ToString());
                    Log.Notice("DataConnecion", "Table " + table.TableName + " doesn't exist, creating...");
                }

                var sb = new StringBuilder();
                var primaryKeys = new Dictionary<string, DataColumn>();
                for (int i = 0; i < table.PrimaryKey.Length; i++)
                {
                    primaryKeys[table.PrimaryKey[i].ColumnName] = table.PrimaryKey[i];
                }

                long IncrementSeed = 0;

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Type systype = table.Columns[i].DataType;

                    string column = "";

                    column += "`" + table.Columns[i].ColumnName + "` ";
                    alterRemoveColumnDefs.Remove(column);

                    if (systype == typeof(char))
                    {
                        column += "SMALLINT UNSIGNED";
                    }
                    else if (systype == typeof(DateTime))
                    {
                        column += "DATETIME";
                    }
                    else if (systype == typeof(sbyte))
                    {
                        column += "TINYINT";
                    }
                    else if (systype == typeof(short))
                    {
                        column += "SMALLINT";
                    }
                    else if (systype == typeof(int))
                    {
                        column += "INT";
                    }
                    else if (systype == typeof(long))
                    {
                        column += "BIGINT";
                    }
                    else if (systype == typeof(byte) || systype == typeof(bool))
                    {
                        column += "TINYINT UNSIGNED";
                    }
                    else if (systype == typeof(ushort))
                    {
                        column += "SMALLINT UNSIGNED";
                    }
                    else if (systype == typeof(uint))
                    {
                        column += "INT UNSIGNED";
                    }
                    else if (systype == typeof(ulong))
                    {
                        column += "BIGINT UNSIGNED";
                    }
                    else if (systype == typeof(float))
                    {
                        column += "FLOAT";
                    }
                    else if (systype == typeof(double))
                    {
                        column += "DOUBLE";
                    }
                    else if (systype == typeof(string))
                    {
                        if (primaryKeys.ContainsKey(table.Columns[i].ColumnName) ||
                            table.Columns[i].ExtendedProperties.ContainsKey("INDEX") ||
                            table.Columns[i].ExtendedProperties.ContainsKey("VARCHAR") ||
                            table.Columns[i].Unique)
                        {
                            if (table.Columns[i].ExtendedProperties.ContainsKey("VARCHAR"))
                            {
                                column += "VARCHAR(" + table.Columns[i].ExtendedProperties["VARCHAR"] + ")";
                            }
                            else
                            {
                                column += "VARCHAR(255)";
                            }
                        }
                        else
                        {
                            column += "TEXT";
                        }
                    }
                    else
                    {
                        column += "TEXT";
                    }
                    if (!table.Columns[i].AllowDBNull)
                    {
                        column += " NOT NULL";
                    }
                    if (table.Columns[i].AutoIncrement)
                    {
                        column += " AUTO_INCREMENT";
                        IncrementSeed = table.Columns[i].AutoIncrementSeed;
                    }

                    columnDefs.Add(column);

                    // Si une colonne n'existe pas dans la table , on l'alter
                    if (currentTableColumns.Count > 0 && !currentTableColumns.Contains(table.Columns[i].ColumnName.ToLower()))
                    {
                        Log.Debug("DataConnecion", "Add alteration " + table.Columns[i].ColumnName.ToLower());
                        alterAddColumnDefs.Add(column);
                    }
                }

                string columndef = string.Join(", ", columnDefs.ToArray());

                // create primary keys
                if (table.PrimaryKey.Length > 0)
                {
                    columndef += ", PRIMARY KEY (";
                    bool first = true;
                    for (int i = 0; i < table.PrimaryKey.Length; i++)
                    {
                        if (!first)
                        {
                            columndef += ", ";
                        }
                        else
                        {
                            first = false;
                        }
                        columndef += "`" + table.PrimaryKey[i].ColumnName + "`";
                    }
                    columndef += ")";
                }

                // Index Unique			
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (table.Columns[i].Unique && !primaryKeys.ContainsKey(table.Columns[i].ColumnName))
                    {
                        columndef += ", UNIQUE INDEX (`" + table.Columns[i].ColumnName + "`)";
                    }
                }

                // Index
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (table.Columns[i].ExtendedProperties.ContainsKey("INDEX")
                        && !primaryKeys.ContainsKey(table.Columns[i].ColumnName)
                        && !table.Columns[i].Unique)
                    {
                        columndef += ", INDEX (`" + table.Columns[i].ColumnName + "`)";
                    }
                }
                sb.Append("CREATE TABLE IF NOT EXISTS `" + table.TableName + "` (" + columndef + ") AUTO_INCREMENT=" + IncrementSeed);

                try
                {
                    Log.Debug("DataConnecion", sb.ToString());

                    ExecuteNonQuery(sb.ToString());
                }
                catch (Exception e)
                {
                    Log.Error("DataConnecion", "Error at creating table " + table.TableName + e.ToString());
                    return;
                }


                // alter table if needed
                if (alterAddColumnDefs.Count > 0)
                {
                    columndef = string.Join(", ", alterAddColumnDefs.ToArray());
                    string alterTable = "ALTER TABLE `" + table.TableName + "` ADD (" + columndef + ")";
                    try
                    {
                        Log.Debug("DataConnecion", alterTable);
                        ExecuteNonQuery(alterTable);
                    }
                    catch (Exception e)
                    {
                        Log.Error("DataConnecion", "Alteration table error " + table.TableName + e.ToString());
                    }
                }

                string ColumnName = table.TableName + "_ID";
                //ExecuteNonQuery("UPDATE `" + table.TableName + "` SET " + ColumnName + " = uuid() WHERE " + ColumnName + " = ''");

                if (alterRemoveColumnDefs.Count > 0)
                {
                    foreach (string Column in alterRemoveColumnDefs)
                    {
                        string alterTable = "ALTER TABLE `" + table.TableName + "` DROP COLUMN " + Column + "";
                        try
                        {
                            Log.Debug("DataConnecion", alterTable);
                            ExecuteNonQuery(alterTable);
                        }
                        catch (Exception e)
                        {
                            Log.Error("DataConnecion", "Alteration table error " + table.TableName + e.ToString());
                        }
                    }
                }
            }
        }

        // Retourne le format des DateTimes
        public string GetDBDateFormat()
        {
            switch (connType)
            {
                case ConnectionType.DATABASE_MYSQL:
                    return "yyyy-MM-dd HH:mm:ss";
            }

            return "yyyy-MM-dd HH:mm:ss";
        }

        // Charge une table a partir de son DataSet
        public void LoadDataSet(string tableName, DataSet dataSet)
        {
            dataSet.Clear();
            switch (connType)
            {
                case ConnectionType.DATABASE_MSSQL:
                    {
                        try
                        {
                            var conn = new SqlConnection(connString);
                            var adapter = new SqlDataAdapter("SELECT * from " + tableName, conn);

                            adapter.Fill(dataSet.Tables[tableName]);
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not load table ", ex);
                        }

                        break;
                    }
                case ConnectionType.DATABASE_ODBC:
                    {
                        try
                        {
                            var conn = new OdbcConnection(connString);
                            var adapter = new OdbcDataAdapter("SELECT * from " + tableName, conn);

                            adapter.Fill(dataSet.Tables[tableName]);
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not load table ", ex);
                        }

                        break;
                    }
                case ConnectionType.DATABASE_OLEDB:
                    {
                        try
                        {
                            var conn = new OleDbConnection(connString);
                            var adapter = new OleDbDataAdapter("SELECT * from " + tableName, conn);

                            adapter.Fill(dataSet.Tables[tableName]);
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not load table ", ex);
                        }
                        break;
                    }
            }
        }

        // Sauvegarde tous les changements effectué dans le dataset
        public void SaveDataSet(string tableName, DataSet dataSet)
        {
            if (dataSet.HasChanges() == false)
                return;

            switch (connType)
            {
                case ConnectionType.DATABASE_MSSQL:
                    {
                        try
                        {
                            var conn = new SqlConnection(connString);
                            var adapter = new SqlDataAdapter("SELECT * from " + tableName, conn);
                            var builder = new SqlCommandBuilder(adapter);

                            adapter.DeleteCommand = builder.GetDeleteCommand();
                            adapter.UpdateCommand = builder.GetUpdateCommand();
                            adapter.InsertCommand = builder.GetInsertCommand();

                            lock (dataSet) // lock dataset to prevent changes to it
                            {
                                adapter.ContinueUpdateOnError = true;
                                DataSet changes = dataSet.GetChanges();
                                adapter.Update(changes, tableName);
                                PrintDatasetErrors(changes);
                                dataSet.AcceptChanges();
                            }

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not save table " + tableName, ex);
                        }

                        break;
                    }
                case ConnectionType.DATABASE_ODBC:
                    {
                        try
                        {
                            var conn = new OdbcConnection(connString);
                            var adapter = new OdbcDataAdapter("SELECT * from " + tableName, conn);
                            var builder = new OdbcCommandBuilder(adapter);

                            adapter.DeleteCommand = builder.GetDeleteCommand();
                            adapter.UpdateCommand = builder.GetUpdateCommand();
                            adapter.InsertCommand = builder.GetInsertCommand();

                            DataSet changes;
                            lock (dataSet) // lock dataset to prevent changes to it
                            {
                                adapter.ContinueUpdateOnError = true;
                                changes = dataSet.GetChanges();
                                adapter.Update(changes, tableName);
                                dataSet.AcceptChanges();
                            }

                            PrintDatasetErrors(changes);

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not save table ", ex);
                        }

                        break;
                    }
                case ConnectionType.DATABASE_MYSQL:
                    {
                        return;
                    }
                case ConnectionType.DATABASE_OLEDB:
                    {
                        try
                        {
                            var conn = new OleDbConnection(connString);
                            var adapter = new OleDbDataAdapter("SELECT * from " + tableName, conn);
                            var builder = new OleDbCommandBuilder(adapter);

                            adapter.DeleteCommand = builder.GetDeleteCommand();
                            adapter.UpdateCommand = builder.GetUpdateCommand();
                            adapter.InsertCommand = builder.GetInsertCommand();

                            DataSet changes;
                            lock (dataSet) // lock dataset to prevent changes to it
                            {
                                adapter.ContinueUpdateOnError = true;
                                changes = dataSet.GetChanges();
                                adapter.Update(changes, tableName);
                                dataSet.AcceptChanges();
                            }

                            PrintDatasetErrors(changes);

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            throw new DatabaseException("Can not save table", ex);
                        }
                        break;
                    }
            }
        }

        // Affiche les erreur du dataset
        public void PrintDatasetErrors(DataSet dataset)
        {
            if (dataset.HasErrors)
            {
                foreach (System.Data.DataTable table in dataset.Tables)
                {
                    if (table.HasErrors)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (row.HasErrors && row.RowState == DataRowState.Deleted)
                            {
                                Log.Error("DataConnection","Error deleting row in table " + table.TableName + ": " + row.RowError);

                                var sb = new StringBuilder();
                                foreach (DataColumn col in table.Columns)
                                {
                                    sb.Append(col.ColumnName + "=" + row[col, DataRowVersion.Original] + " ");
                                }

                                Log.Error("DataConnection", sb.ToString());
                            }
                            else if (row.HasErrors)
                            {
                                Log.Error("DataConnection", "Error updating table " + table.TableName + ": " + row.RowError + row.GetColumnsInError());

                                var sb = new StringBuilder();
                                foreach (DataColumn col in table.Columns)
                                {
                                    sb.Append(col.ColumnName + "=" + row[col] + " ");
                                }

                                Log.Error("DataConnection", sb.ToString());

                                sb = new StringBuilder("Affected columns: ");
                                foreach (DataColumn col in row.GetColumnsInError())
                                {
                                    sb.Append(col.ColumnName + " ");
                                }

                                Log.Error("DataConnection", sb.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}