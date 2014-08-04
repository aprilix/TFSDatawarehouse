using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TFS.Warehouse.Adapter.Model
{
    public class TFSUsers
    {
        public string UserName { get; set; }

        public string UserAgent { get; set; }

        public DateTime AccessDate { get; set; }

        public TFSUsers(string usuario, string userAgent, DateTime accessDate)
        {
            UserName    = usuario;
            UserAgent  = userAgent;
            AccessDate = accessDate;
        }
        
    }
}
