using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Warehouse.Adapter.Model
{
    public class TblCommand
    {
        public string IdentityName { get; set; }
        public string UserAget { get; set; }
        public DateTime StartDate { get; set; }
        public long Quantidade;
    }
}
