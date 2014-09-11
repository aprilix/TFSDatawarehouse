using System;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Warehouse;
using TFS.Warehouse.Adapter.Infra;
using TFS.Warehouse.Adapter.Services;
using TFS.Warehouse.Adapter.VersionHelper;


namespace TFS.Warehouse.Adapter
{
    public class TFSUsersAdapterJobExtension : WarehouseSyncJobExtension<TFSUsersAdapter> { }
    public class TFSUsersAdapter : WarehouseAdapter
    {
        private static string dateOfLastCommandSavedProperty = "/Adapter/Schema/TFSUsersAdapter/DateOfLastCommandSaved";


        private enum AdapterState
        {
            Stopped,
            Initializing,
            Processing
        }

        private AdapterState _adapterState;
        private IVersionHelper _warehouseVersion;
        private string _tfsLogUsageConnString;
        private Database _tpcDatabase;

        public override void Initialize()
        {
            _warehouseVersion = new VersionHelper.TFSUsersAdapterVersion();

            _tpcDatabase = Infra.TFSUtils.RetrieveTPCConectionString(this.RequestContext);

            _tfsLogUsageConnString = _tpcDatabase.ConnectionString.Replace(_tpcDatabase.DatabaseName, "TFS_CustomDataWarehouse");

            _adapterState = AdapterState.Stopped;
        }

        public override DataChangesResult MakeDataChanges()
        {
            //System.Diagnostics.Debugger.Launch();

            try
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

                    TFSUsersLogUpdater.UpdateTFSUsersLog(TFSUtils.ReturnDataFromWarehousePropertyBag<DateTime>(dac, dateOfLastCommandSavedProperty), _tpcDatabase.ConnectionString, _tfsLogUsageConnString);
                }

                return ChangeAdapterState(DataChangesResult.NoChangesPending);
            }
            catch (Exception err)
            {
                Utils.Log.LogEvent(RequestContext, err.Message, Utils.Log.LogEventInformationLevel.Error);
                throw;
            }
        }

        public override void MakeSchemaChanges()
        {
            try
            {
                var updater = new DatabaseUpdater<TFSUsersContext>(_tfsLogUsageConnString);

                updater.CreateDatabaseIfNotExists();

                using (var dac = WarehouseContext.CreateWarehouseDataAccessComponent())
                {
                    _warehouseVersion.SetWarehouseVersion(dac);
                }
            }
            catch (Exception err)
            {
                Utils.Log.LogEvent(RequestContext, err.Message, Utils.Log.LogEventInformationLevel.Error);
                throw;
            }
        }

        private DataChangesResult ChangeAdapterState(DataChangesResult result)
        {
            _adapterState = AdapterState.Stopped;

            return result;
        }
    }
}
