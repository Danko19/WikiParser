using System;
using System.IO;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;

namespace WikiParser.MySql
{
    public class MySqlStorageBase : IDisposable
    {
        protected readonly DbManager Db;
        protected MySqlStorageBase ()
        {
            Db = new DbManager(new MySqlDataProvider(), connectionString);
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        private static readonly string connectionString = File.ReadAllText("mySqlSettings");
    }
}