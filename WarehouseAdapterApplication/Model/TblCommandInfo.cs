using System;

namespace TFS.Warehouse.Adapter.Model
{
    public class TblCommandInfo
    {
        public string IdentityName { get; set; }
        public string UserAgent { get; set; }
        public DateTime StartTime { get; set; }
        public int Quantity { get; set; }
    }
}
