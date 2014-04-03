using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.IO;
using ShewhartCards.Dao;


namespace ShewhartCards.Services
{
    class DataStorageService
    {
        private const string CREATE_TABLE_SELECTION_QUERY = @"CREATE TABLE selection([id] INTEGER PRIMARY KEY AUTOINCREMENT,[name] TEXT NOT NULL);";
        private const string CREATE_TABLE_SUBGROUP_QUERY = @"CREATE TABLE subgroup([id] INTEGER PRIMARY KEY AUTOINCREMENT,[selection_id] INTEGER NOT NULL,[group_size] INTEGER NOT NULL, FOREIGN KEY (selection_id) references selection(id) ON DELETE CASCADE);";
        private const string CREATE_TABLE_VALUE_QUERY = @"CREATE TABLE value([id] INTEGER PRIMARY KEY AUTOINCREMENT,[subgroup_id] INTEGER NOT NULL,[value] REAL NOT NULL, FOREIGN KEY (subgroup_id) references subgroup(id) ON DELETE CASCADE);";
        private const string DATABASE_FILE_NAME = @"test_values_collection.db";

        private SqliteDao DBController;
        private string path;

        public DataStorageService()
        {
            DBController = new SqliteDao();
            path = getDataBaseFilePath();
            DBController.executeNonQuery(path, CREATE_TABLE_SELECTION_QUERY, 1);
            DBController.executeNonQuery(path, CREATE_TABLE_SUBGROUP_QUERY, 1);
            DBController.executeNonQuery(path, CREATE_TABLE_VALUE_QUERY, 1);
        }

        /// <summary>
        /// Добавляет в базу данных новую выборку с именем selectionName, либо ссылается на ранее добавленную.
        /// </summary>
        /// <param name="selectionName">Уникальное имя выборки.</param>
        /// <returns>Возвращает id новой, либо ранее добавленной выборки.</returns>
        public long addSelection(string selectionName)
        {
            long selectionId;
            if (isTestExisting(selectionName))
            {
                selectionId = findSelectionByName(selectionName);
            }
            else
            {
                string insertIntoTestQuery = @"INSERT INTO selection(name) values('" + selectionName + "')";
                selectionId = DBController.executeNonQuery(path, insertIntoTestQuery, 1);
            }
            return selectionId;
        }

        /// <summary>
        /// Добавялет в базу данных новую подгруппу вычислений для конкретной выборки.
        /// </summary>
        /// <param name="selectionId">Id выборки.</param>
        /// <param name="valuesQuantity">Размер подгруппы.</param>
        /// <returns>Id подгруппы.</returns>
        public long addSubgroup(long selectionId, int valuesQuantity)
        {
            string query = @"insert into subgroup(selection_id,group_size)";
            query += " values(" + selectionId + "," + valuesQuantity + ");";
            long subgroupId = DBController.executeNonQuery(path, query, 1);
            return subgroupId;
        }

        /// <summary>
        /// Добавялет в базу выборочные значения для конкретной подгруппы.
        /// </summary>
        /// <param name="subgroupId">Id подгруппы</param>
        /// <param name="values">Массив добавляемых значений</param>
        public void addValues(long subgroupId, double[] values)
        {
            StringBuilder insertInValueQueryBuilder = new StringBuilder(@"insert into value(subgroup_id,value) values");
            for (int i = 0; i < values.Length; i++)
            {
                if (i != values.Length - 1)
                {
                    insertInValueQueryBuilder.Append(" (" + subgroupId + "," + values[i].ToString(CultureInfo.InvariantCulture) + "),");
                }
                else
                {
                    insertInValueQueryBuilder.Append(" (" + subgroupId + "," + values[i].ToString(CultureInfo.InvariantCulture) + ");");
                }
            }
            DBController.executeNonQuery(path, insertInValueQueryBuilder.ToString(), 1);
        }

        /// <summary>
        /// Удаляет выборку со всеми данными.
        /// </summary>
        /// <param name="selectionName">Id выборки.</param>
        public void deleteSelection(string selectionName)
        {
            string query = @"DELETE FROM 'selection' WHERE name='" + selectionName + "';";
            DBController.executeNonQuery(path, query, 1);
        }

        /// <summary>
        /// Получить все значения выборки с именем selectionName.
        /// </summary>
        /// <param name="selectionName">Уникальное имя выборки.</param>
        /// <returns>Массив значений выборки - если значения есть.
        ///          null - если значений нету.
        /// </returns>
        public double[] getSelectionsValues(string selectionName)
        {
            long selectionId = findSelectionByName(selectionName);
            string query = @"SELECT v.value FROM 'selection' s INNER JOIN 'subgroup' sg ON s.id = sg.selection_id INNER JOIN 'value' v ON sg.id = v.subgroup_id WHERE s.name='" + selectionName + "';";
            DataRow[] dataRows = DBController.execute(path, query);
            double[] values;
            if (dataRows == null || dataRows.Length == 0)
            {
                values = null;
            }
            else
            {
                values = new double[dataRows.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = Convert.ToDouble(dataRows[i]["value"]);
                }
            }
            return values;
        }

        public int[][] getSelectionsGroupsOfDeviations(string selectionName)
        {
            long selectionId = findSelectionByName(selectionName);
            string query = @"SELECT sg.group_size,v.value FROM 'selection' s INNER JOIN 'subgroup' sg ON s.id = sg.selection_id INNER JOIN 'value' v ON sg.id = v.subgroup_id WHERE s.name='" + selectionName + "';";
            DataRow[] dataRows = DBController.execute(path, query);
            int[][] values;
            if (dataRows == null || dataRows.Length == 0)
            {
                values = null;
            }
            else
            {
                values = new int[dataRows.Length][];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = new int[2];
                    values[i][0] = Convert.ToInt32(dataRows[i]["group_size"]);
                    values[i][1] = Convert.ToInt32(dataRows[i]["value"]);
                }
            }
            return values;
        }

        /// <summary>
        /// Удалить значение value и все связанные с ним подгруппы в выборке с именем selectionName.
        /// </summary>
        /// <param name="selectionName">Уникальное имя выборки.</param>
        /// <param name="value">Удаляемое значение.</param>
        public void deleteValueAndItsGroups(string selectionName, double value)
        {
            string query = @"DELETE FROM 'subgroup' WHERE subgroup.id IN (
                                SELECT sg.id FROM 'subgroup' sg
                                    INNER JOIN 'value' v ON v.subgroup_id=sg.id
                                    INNER JOIN 'selection' s ON s.id=sg.selection_id 
                                    WHERE v.value=" + value.ToString(CultureInfo.InvariantCulture) + " AND s.name='" + selectionName + "');";
            DBController.executeNonQuery(path, query, 1);
        }

        public int[] getValuesQuantityForEachGroup(long selectionId)
        {
            string query = @"SELECT sg.group_size FROM subgroup sg INNER JOIN selection c ON c.id = sg.selection_id WHERE c.id = " + selectionId + ";";
            DataRow[] dataRows = DBController.execute(path, query);
            int[] values;
            if (dataRows == null || dataRows.Length == 0)
            {
                values = null;
            }
            else
            {
                values = new int[dataRows.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = Convert.ToInt32(dataRows[i]["group_size"]);
                }
            }
            return values;
        }

        private string getDataBaseFilePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FILE_NAME);
        }

        private long findSelectionByName(string selectionName)
        {
            string sql = @"SELECT s.id FROM selection s WHERE s.name='" + selectionName + "';";
            DataRow[] dataRows = DBController.execute(path, sql);
            if (dataRows.Length == 0)
            {
                return -1;
            }
            return Convert.ToInt64(dataRows[0]["id"]);
        }

        private bool isTestExisting(string selectionName)
        {
            long selectionId = findSelectionByName(selectionName);
            if (selectionId == -1L)
            {
                return false;
            }
            return true;
        }
    }
}

