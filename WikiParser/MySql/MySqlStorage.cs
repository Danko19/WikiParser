using System.Collections.Generic;
using System.Reflection;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;

namespace WikiParser.MySql
{
    public class MySqlStorage : MySqlStorageBase
    {
        public List<TItem> GetAll<TItem>()
        {
            var name = (typeof(TItem).GetCustomAttribute(typeof(TableNameAttribute)) as TableNameAttribute)?.Name;
            return Db.SetCommand($"SELECT * FROM {name}").ExecuteList<TItem>();
        }

        public void Insert<TItem>(TItem item)
        {
            Db.Insert(item);
        }

        public void InsertMany<TITem>(IEnumerable<TITem> items)
        {
            Db.InsertBatch(items);
        }

        public void Update<TItem>(TItem item)
        {
            Db.Update(item);
        }

        public void UpdateMany<TItem>(IEnumerable<TItem> items)
        {
            Db.Update(items);
        }

        public void InsertOrUpdate<TItem>(TItem item)
        {
            Db.InsertOrReplace(item);
        }

        public void InsertOrUpdateMany<TItem>(IEnumerable<TItem> item)
        {
            Db.InsertOrReplace(item);
        }

        public void Delete<TItem>(TItem item)
        {
            Db.Delete(item);
        }

        public void DeleteMany<TItem>(IEnumerable<TItem> items)
        {
            Db.Delete(items);
        }
    }
}