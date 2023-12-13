using System.Collections.Generic;
using UnityEngine;

namespace VoidexSoft.Inventory.DataCore
{
    /// <summary>
    /// A static class that provides access to the <see cref="Database"/> asset globally.
    /// </summary>
    public static class DatabaseManager
    {
        public static Database DB
        {
            get
            {
                if (s_db == null) FindDatabase();
                return s_db;
            }
            set => s_db = value;
        }
        private static Database s_db;
        
        public static DataEntity Get(int id)
        {
            return DB.Get(id);
        }
        
        public static List<T> GetAll<T>() where T : DataEntity
        {
            return DB.GetAll<T>();
        }

        private static void FindDatabase()
        {
            DB = (Database)Resources.Load(Database.DATABASE_NAME);
        }
    }
}