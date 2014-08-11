using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Warehouse.Adapter.Infra
{
    public class DatabaseUpdater<T> where T : DbContext
    {
        private string _connString {get; set;}
        public DatabaseUpdater(string connString)
        {
            _connString = connString;
        }


        public void CreateDatabaseIfNotExists()
        {
            using (var dbContext = (T)Activator.CreateInstance(typeof(T), _connString))
            { 
                dbContext.Database.CreateIfNotExists();
            }
        }
    }
}
