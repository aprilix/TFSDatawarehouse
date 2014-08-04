using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Warehouse.Adapter.Model;

namespace TFS.Warehouse.Adapter.DataAccessComponent
{
    public class TblCommandContext : DbContext
    {
        public TblCommandContext(string connectionString) : base("ConnectionString=" + connectionString) { }

        public override int SaveChanges()
        {
            throw new InvalidOperationException();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblCommand>();
        }
    }
}
