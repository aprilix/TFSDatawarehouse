using System;
using System.Reflection;
using Microsoft.TeamFoundation.Warehouse;

namespace TFS.Warehouse.Adapter.VersionHelper
{
    public class TFSUsersAdapterVersion : IVersionHelper
    {
        public string VersionPropertyBagKey
        {
            get { return "/Adapter/Schema/TFSUsersAdapter/WarehouseSchemaVersion"; }
        }

        public bool NeedSchemaChanges(Microsoft.TeamFoundation.Warehouse.WarehouseDataAccessComponent dac)
        {
            var storedProperty = dac.GetProperty(null, VersionPropertyBagKey);

            Version installedVersion;

            if (Version.TryParse(storedProperty, out installedVersion))
            {
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                var compare = installedVersion.CompareTo(assemblyVersion);

                if (compare > 0) throw new WarehouseException(string.Format("Não é possível atualizar a versão do Adapter. Ultima versão aplicada na base: {0} -> Versão do Assembly: {1}", installedVersion, assemblyVersion));
                if (compare == 0) return false;
                if (compare < 0) return true;
            }

            return true;
        }

        public void SetWarehouseVersion(Microsoft.TeamFoundation.Warehouse.WarehouseDataAccessComponent dac)
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            dac.SetProperty(null, VersionPropertyBagKey, assemblyVersion.ToString());
        }


    }
}
