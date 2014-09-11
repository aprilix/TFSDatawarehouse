using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TFS.Warehouse.Adapter.Infra;
using TFS.Warehouse.Adapter.Model;

namespace TFS.Warehouse.Adapter.Services
{
    public class TFSUsersLogUpdater
    {
        //private static string dateOfLastCommandSavedProperty = "/Adapter/Schema/TFSUsersAdapter/DateOfLastCommandSaved";

        private static string sql = new StringBuilder()
                                    .AppendLine("SELECT Max(StartTime) StartTime, IdentityName, UserAgent, Count(IdentityName) Quantity")
                                    .AppendLine("  FROM tbl_Command")
                                    .AppendLine(" WHERE StartTime >= @StartTime")
                                    .AppendLine(" GROUP BY Convert(date,StartTime), IdentityName, UserAgent")
                                    .AppendLine(" ORDER BY Convert(date,StartTime)").ToString();

        /// <summary>
        /// Faz o update ou insert de dados nas tabelas de log
        /// </summary>
        /// <param name="dateOfLastCommandSaved">Data dos ultimos comandos processados pelo Adapter</param>
        /// <param name="_tpcDatabaseConnString">ConnectionString para o banco de dados do Team Project Collection</param>
        /// <param name="_tfsLogUsageConnString">ConnectionString para o banco de dadaos do Log</param>
        /// <returns>Data do ultimo comando processado</returns>
         public static DateTime UpdateTFSUsersLog(DateTime dateOfLastCommandSaved, string _tpcDatabaseConnString, string _tfsLogUsageConnString)
        {
            var commands = new List<TblCommandInfo>();

            using (var collectionDb = new TFSCollectionContext(_tpcDatabaseConnString))
            {
            //    var dateOfLastCommandSaved = dac.GetProperty(null, dateOfLastCommandSavedProperty);

            //    if (string.IsNullOrEmpty(dateOfLastCommandSaved))
            //    {
            //        dateOfLastCommandSaved = "01/01/2001";
            //    }

                commands = collectionDb.Database.SqlQuery<TblCommandInfo>(sql, new SqlParameter("@StartTime", dateOfLastCommandSaved.Date)).ToList();
            }

            //using (var tfsUsersContext = new TFSUsersContext("Data Source=.;Initial Catalog=TFS_CustomDataWarehouse;Integrated Security=SSPI;"))
            using (var tfsUsersContext = new TFSUsersContext(_tfsLogUsageConnString))            
            {
                commands.ForEach(x => 
                {
                    var tfsUserLog = tfsUsersContext.TFSUsageLog.FirstOrDefault(u => u.UserAgent == x.UserAgent && u.UserName == x.IdentityName && DbFunctions.TruncateTime(u.AccessDate) == DbFunctions.TruncateTime(x.StartTime));

                    if (tfsUserLog != null)
                    {
                        tfsUserLog.Quantity = x.Quantity;
                        tfsUserLog.AccessDate = x.StartTime;
                    }
                    else
                    {
                        var tfsUsageLog = new TFSUsageLog(x.IdentityName, x.StartTime, x.Quantity);
                        tfsUsageLog.SetSoftwareUsedToConnect(x.UserAgent);

                        tfsUsersContext.TFSUsageLog.Add(tfsUsageLog);
                    }
                });

                tfsUsersContext.SaveChanges();
            }

            return commands.Max(x => x.StartTime);
            //dac.SetProperty(null, dateOfLastCommandSavedProperty, commands.Max(x => x.StartTime).ToString());
        }
    }
}
