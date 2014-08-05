using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Warehouse.Adapter.Infra;
using TFS.Warehouse.Adapter.Model;
using System.Data.Entity;

namespace TFS.Warehouse.Adapter.Services
{
    public class TFSUsersLogUpdater
    {
        private static string dateOfLastCommandSavedProperty = "/Adapter/Schema/TFSUsersAdapter/DateOfLastCommandSaved";

        private static string sql = new StringBuilder()
                                    .AppendLine("SELECT Max(StartTime) StartTime, IdentityName, UserAgent, Count(IdentityName) Quantity")
                                    .AppendLine("  FROM tbl_Command")
                                    .AppendLine(" WHERE StartTime >= @StartTime")
                                    .AppendLine(" GROUP BY Convert(date,StartTime), IdentityName, UserAgent")
                                    .AppendLine(" ORDER BY Convert(date,StartTime)").ToString();

        public static void UpdateTFSUsersLog(WarehouseDataAccessComponent dac, TeamFoundationRequestContext requestContext)
        {
            var tfsRegistration = GetRegistrationService(requestContext);

            var entries = tfsRegistration.GetRegistrationEntries("vstfs").Select(x => x.Databases.FirstOrDefault(y => y.Name == "BIS DB"));

            var connString = "";
            if (entries.Any())
                connString = entries.First().ConnectionString;
            else
                throw new ApplicationException("Não foi possível determinar a string de conexao com o Banco de Dados da Collection");

            var commands = new List<TblCommandInfo>();

            using (var collectionDb = new TFSCollectionContext(connString))
            {
                var dateOfLastCommandSaved = dac.GetProperty(null, dateOfLastCommandSavedProperty);

                if (string.IsNullOrEmpty(dateOfLastCommandSaved))
                {
                    dateOfLastCommandSaved = "01/01/2001";
                }

                commands = collectionDb.Database.SqlQuery<TblCommandInfo>(sql, new SqlParameter("@StartTime", DateTime.Parse(dateOfLastCommandSaved).Date)).ToList();
            }

            using (var tfsUsersContext = new TFSUsersContext("Data Source=.;Initial Catalog=TFS_CustomDataWarehouse;Integrated Security=SSPI;"))
            {
                commands.ForEach(x => 
                {
                    var tfsUserLog = tfsUsersContext.TFSUsers.FirstOrDefault(u => u.UserAgent == x.UserAgent && u.UserName == x.IdentityName && DbFunctions.TruncateTime(u.AccessDate) == DbFunctions.TruncateTime(x.StartTime));

                    if (tfsUserLog != null)
                    {
                        tfsUserLog.Quantity = x.Quantity;
                        tfsUserLog.AccessDate = x.StartTime;
                    }
                    else
                    { 
                        tfsUsersContext.TFSUsers.Add(new TFSUsers(x.IdentityName, x.UserAgent, x.StartTime, x.Quantity));
                    }
                });

                tfsUsersContext.SaveChanges();
            }

            dac.SetProperty(null, dateOfLastCommandSavedProperty, commands.Max(x => x.StartTime).ToString());
        }

        private static IRegistration GetRegistrationService(TeamFoundationRequestContext requestContext)
        {
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            var tfsUri = new Uri(locationService.GetServerAccessMapping(requestContext).AccessPoint + "/" + requestContext.ServiceHost.Name);

            var teamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsUri);

            return teamProjectCollection.GetService<IRegistration>();
        }
    }
}
