using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Warehouse.Adapter.Model;

namespace TFS.Warehouse.Adapter.Infra
{
    public class TFSCollectionContext : DbContext
    {
        public TFSCollectionContext(string connectionString) : base(connectionString) { }
  
        public DbQuery<TblCommandInfo> Commands
        {
            get
            {
                return Set<TblCommandInfo>().AsNoTracking();
            }
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is readonly!");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TFSCollectionContext>(null);
        }
    }
}
