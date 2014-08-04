using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Warehouse;

namespace TFS.Warehouse.Adapter
{
    public class TFSUsersAdapterJobExtension : WarehouseSyncJobExtension<TFSUsersAdapter> { }

    public class TFSUsersAdapter : WarehouseAdapter
    {
        private enum AdapterState
        { 
            Stopped,
            Initializing,
            Processing
        }

        private AdapterState _adapterState;

        public override void Initialize()
        {
            _adapterState = AdapterState.Stopped;
        }

        public override DataChangesResult MakeDataChanges()
        {
            if (_adapterState != AdapterState.Stopped)
                return DataChangesResult.NoChangesPending;

            return ChangeAdapterState(DataChangesResult.NoChangesPending);
        }

        public override void MakeSchemaChanges()
        {
            ChangeAdapterState(DataChangesResult.NoChangesPending);
        }

        private DataChangesResult ChangeAdapterState(DataChangesResult result)
        {
            _adapterState = AdapterState.Stopped;

            return result;
        }
    }
}
