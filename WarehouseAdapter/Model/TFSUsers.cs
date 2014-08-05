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
    public class TFSUsers
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string UserAgent { get; set; }

        public DateTime AccessDate { get; set; }

        public int Quantity { get; set; }

        public TFSUsers(string userName, string userAgent, DateTime accessDate, int quantity)
        {
            UserName    = userName;
            UserAgent   = userAgent;
            AccessDate  = accessDate;
            Quantity    = quantity;
        }
    }
}
