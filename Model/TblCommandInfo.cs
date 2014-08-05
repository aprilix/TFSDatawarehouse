using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Warehouse.Adapter.Model
{
    public class TblCommandInfo
    {
        public string UserName { get; set; }
        public string UserAgent { get; set; }
        public DateTime StartTime { get; set; }
        public int Quantity { get; set; }
    }
}
