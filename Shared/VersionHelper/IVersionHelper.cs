using Microsoft.TeamFoundation.Warehouse;

namespace TFS.Warehouse.Adapter.VersionHelper
{
    public interface IVersionHelper
    {
        string VersionPropertyBagKey { get; }

        bool NeedSchemaChanges(WarehouseDataAccessComponent dac);

        void SetWarehouseVersion(WarehouseDataAccessComponent dac);
    }
}
