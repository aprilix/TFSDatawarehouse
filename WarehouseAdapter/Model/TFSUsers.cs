using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.TeamFoundation.Warehouse;
using TFS.Warehouse.Adapter.Infra;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Client;

namespace TFS.Warehouse.Adapter.Model
{
    public class TFSUsageLog
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Use method SetSoftwareUsedToConnect to set this property
        /// </summary>
        public string UserAgent { get; protected set; }

        public DateTime AccessDate { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Use method SetSoftwareUsedToConnect to set this property
        /// </summary>
        public string Software { get; protected set; }

        public TFSUsageLog(string userName, DateTime accessDate, int quantity)
        {
            UserName = userName;
            AccessDate = accessDate;
            Quantity = quantity;
        }

        public TFSUsageLog() { }

        public void SetSoftwareUsedToConnect(string userAgent)
        {
            UserAgent = userAgent;

            userAgent = userAgent.ToUpper();

            if (userAgent.Contains("MOZILLA"))
            {
                Software = "Browser";
            }
            else if (userAgent.Contains("EVERYWHERE"))
            {
                Software = "Team Explorer Everywhere";
            }
            else if (userAgent.Contains("DEVENV"))
            {
                if (userAgent.Contains("9."))
                    Software = "Visual Studio 2008";
                else if (userAgent.Contains("10."))
                    Software = "Visual Studio 2010";
                else if (userAgent.Contains("11."))
                    Software = "Visual Studio 2012";
                else if (userAgent.Contains("12."))
                    Software = "Visual Studio 2013";
                else
                    Software = "Visual Studio";
            }
            else if (userAgent.Contains("MTM.EXE"))
            {
                Software = "Test Manager";
            }
            else if (userAgent.Contains("EXCEL") || userAgent.Contains("WORD") || userAgent.Contains("OFFICE"))
            {
                Software = "Microsoft Office";
            }
            else
            {
                Software = "Outros";
            }
        }
    }
}
