using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Warehouse.Adapter.Model;

namespace TFS.Warehouse.Adapter.Infra
{
    public class TFSUsersContext : DbContext
    {
        public DbSet<TFSUsers> TFSUsers;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Types<TFSUsers>()
                .Configure(x =>
                {
                    x.Property(p => p.Id).IsKey().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                    x.Property(p => p.UserName).HasMaxLength(50);
                    x.Property(p => p.UserAgent).HasMaxLength(200);
                    x.Property(p => p.AccessDate).HasColumnType("DateTime");
                });
        }
    }
}
