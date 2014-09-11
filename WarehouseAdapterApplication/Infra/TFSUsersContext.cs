using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TFS.Warehouse.Adapter.Model;

namespace TFS.Warehouse.Adapter.Infra
{
    public class TFSUsersContext : DbContext
    {
        public TFSUsersContext() : base() { }

        public TFSUsersContext(string connString) : base(connString) { }

        public DbSet<TFSUsageLog> TFSUsageLog {get;set;}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Types<TFSUsageLog>()
                .Configure(x =>
                {
                    x.Property(p => p.Id).IsKey().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    x.Property(p => p.UserName).HasMaxLength(50);
                    x.Property(p => p.UserAgent).HasMaxLength(200);
                    x.Property(p => p.AccessDate).HasColumnType("DateTime");
                });
        }
    }

    public class TFSUserContextUpdater
    {
        public void CreateDatabaseIfNotExists()
        { 
            
        }
    }
}
