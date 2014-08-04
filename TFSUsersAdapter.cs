using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Warehouse;
using TFS.Warehouse.Adapter.VersionHelper;
using TFS.Warehouse.Adapter.DataAccessComponent;
using Microsoft.TeamFoundation.Framework.Server;

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
        private IVersionHelper _warehouseVersion;

        public override void Initialize()
        {
            _warehouseVersion = new VersionHelper.TFSUsersAdapterVersion();

            _adapterState = AdapterState.Stopped;
        }

        public override DataChangesResult MakeDataChanges()
        {
            if (_adapterState != AdapterState.Stopped)
                return DataChangesResult.NoChangesPending;

            if (IsWarehouseHostCancelled || IsWarehouseSchemaLockRequested)
            {
                return ChangeAdapterState(DataChangesResult.DataChangesPending);
            }
           
            using (var dac = WarehouseContext.CreateWarehouseDataAccessComponent())
            {
                if (_warehouseVersion.NeedSchemaChanges(dac))
                {
                    return ChangeAdapterState(DataChangesResult.SchemaChangesPending);
                }

                var tfsDatabaseSettings = RequestContext.GetService<TeamFoundationDatabaseSettings>();
                               
            }

            return ChangeAdapterState(DataChangesResult.NoChangesPending);
        }

        public override void MakeSchemaChanges()
        {
            try
            {
                using (var db = new TFSUsersContext())
                {
                    db.Database.CreateIfNotExists();
                }

                using (var dac = WarehouseContext.CreateWarehouseDataAccessComponent())
                {
                    _warehouseVersion.SetWarehouseVersion(dac);
                }
            }
            catch (Exception err)
            {
                Utils.Log.LogEvent(RequestContext, err.Message, Utils.Log.LogEventInformationLevel.Error);
            }
        }   

        private DataChangesResult ChangeAdapterState(DataChangesResult result)
        {
            _adapterState = AdapterState.Stopped;

            return result;
        }
    }
}
