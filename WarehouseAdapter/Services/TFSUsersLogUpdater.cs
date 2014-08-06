﻿using Microsoft.TeamFoundation.Client;
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

         internal static void UpdateTFSUsersLog(WarehouseDataAccessComponent dac, TeamFoundationRequestContext requestContext, string _tpcDatabaseConnString, string _tfsLogUsageConnString)
        {
            var commands = new List<TblCommandInfo>();

            using (var collectionDb = new TFSCollectionContext(_tpcDatabaseConnString))
            {
                var dateOfLastCommandSaved = dac.GetProperty(null, dateOfLastCommandSavedProperty);

                if (string.IsNullOrEmpty(dateOfLastCommandSaved))
                {
                    dateOfLastCommandSaved = "01/01/2001";
                }

                commands = collectionDb.Database.SqlQuery<TblCommandInfo>(sql, new SqlParameter("@StartTime", DateTime.Parse(dateOfLastCommandSaved).Date)).ToList();
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

            dac.SetProperty(null, dateOfLastCommandSavedProperty, commands.Max(x => x.StartTime).ToString());
        }
    }
}
