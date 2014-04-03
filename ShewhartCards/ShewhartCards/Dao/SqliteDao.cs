using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace ShewhartCards.Dao
{
    class SqliteDao
    {
        public SqliteDao()
        {
        }

        /// <summary>
        /// Выполнить запрос без выборки данных.
        /// </summary>
        /// <param name="FileData"> Название файла БД + путь к нему. </param>
        /// <param name="sql"> Тело запроса </param>
        /// <param name="where">
        ///     0 - если БД нету, то создадим;
        ///     1 - если БД нету, то не создадим.
        /// </param>
        /// <returns>Колличество затронутых записей</returns>
        public long executeNonQuery(string FileData, string sql, int where)
        {
            long lastInsertRowId;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    if (where == 0)
                    {
                        con.ConnectionString = @"Data Source=" + FileData + ";New=True;Version=3";
                    }
                    else
                    {
                        con.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    }
                    con.Open();

                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        enableCascadeDeleting(sqlCommand);
                        sqlCommand.CommandText = sql;
                        sqlCommand.ExecuteNonQuery();
                        lastInsertRowId = getLastInsertRowId(sqlCommand);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                lastInsertRowId = 0L;
            }
            return lastInsertRowId;
        }

        /// <summary>
        /// Активирует каскадное удаление связанных записей;
        /// </summary>
        /// <param name="sqlCommand"></param>
        private void enableCascadeDeleting(SQLiteCommand sqlCommand)
        {
            sqlCommand.CommandText = @"PRAGMA foreign_keys = ON;";
            sqlCommand.ExecuteScalar();
        }

        /// <summary>
        /// Функция позволяет узнать id последней добавленной записи.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns>id последней добавленной записи</returns>
        private long getLastInsertRowId(SQLiteCommand sqlCommand)
        {
            sqlCommand.CommandText = @"SELECT last_insert_rowid()";
            return (long)sqlCommand.ExecuteScalar();
        }

        /// <summary>
        /// Выполнить запрос с выборкой данных.
        /// </summary>
        /// <param name="FileData"> Название файла БД + путь к нему. </param>
        /// <param name="sql"> Тело запроса </param>
        /// <returns>Записи</returns>
        public DataRow[] execute(string FileData, string sql)
        {
            DataRow[] dataRows = null;
            SQLiteDataAdapter dataAdapter = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection())
                {
                    connection.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    connection.Open();
                    using (SQLiteCommand sqlCommand = connection.CreateCommand())
                    {
                        dataAdapter = new SQLiteDataAdapter(sql, connection);
                        dataSet.Reset();
                        dataAdapter.Fill(dataSet);
                        dataTable = dataSet.Tables[0];
                        dataRows = dataTable.Select();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                dataRows = null;
            }
            return dataRows;

        }
    }
}
