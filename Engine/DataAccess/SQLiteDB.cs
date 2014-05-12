using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SpeechRecognition.DataAccess
{
    public class SQLiteDB
    {
        SQLiteConnection cnn;
        string DbConnection;

        public SQLiteDB()
        {            
            DbConnection = String.Format("Data Source=db.sqlite");
        }

        public SQLiteDB(string inputfile)
        {
            DbConnection = String.Format("Data Source={0}", inputfile);
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(DbConnection);
                cnn.Open();
                SQLiteCommand cmd = new SQLiteCommand(cnn);
                cmd.CommandText = sql;
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex.Message);
            }

            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(DbConnection);
            cnn.Open();
            SQLiteCommand cmd = new SQLiteCommand(cnn);
            cmd.CommandText = sql;
            int rowsUpdated = cmd.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(DbConnection);
            cnn.Open();
            SQLiteCommand cmd = new SQLiteCommand(cnn);
            cmd.CommandText = sql;
            object val = cmd.ExecuteScalar();
            cnn.Close();

            if (val != null)
                return val.ToString();
            else
                return String.Empty;
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            string vals = "";
            bool returnCode = true;

            if (data.Count > 0)
            {
                foreach (KeyValuePair<string, string> val in data)
                    vals += String.Format(" {0} = '{1}',",val.Key.ToString(), val.Value.ToString());
                vals = vals.Substring(0, vals.Length - 1);
            }

            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(string tableName, string where)
        {
            bool returnCode = true;
            
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1}", tableName, where));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<string, string> data)
        {
            bool returnCode = true;
            string columns = "", values = "";

            foreach (KeyValuePair<string, string> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", (val.Value != null ? val.Value.Replace("'", "''") : val.Value));
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);

            try
            {
                this.ExecuteNonQuery(String.Format("insert or replace into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            bool returnCode = true;
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");

                foreach (DataRow row in tables.Rows)
                {
                    this.ClearTable(row["NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(string table)
        {
            bool returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0};", table));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnCode = false;
            }

            return returnCode;
        }
    }
}
