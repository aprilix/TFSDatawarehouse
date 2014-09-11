using System;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Warehouse;

namespace TFS.Warehouse.Adapter.Infra
{
    public class TFSUtils
    {
        public static Database RetrieveTPCConectionString(TeamFoundationRequestContext requestContext)
        {
            var tfsRegistration = GetRegistrationService(requestContext);

            var entries = tfsRegistration.GetRegistrationEntries("vstfs").Select(x => x.Databases.FirstOrDefault(y => y.Name == "BIS DB"));

            if (entries.Any())
                return entries.First();
            else
                throw new ApplicationException("Não foi possível determinar a string de conexao com o Banco de Dados da Collection");
        }

        private static IRegistration GetRegistrationService(TeamFoundationRequestContext requestContext)
        {
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            var tfsUri = new Uri(locationService.GetServerAccessMapping(requestContext).AccessPoint + "/" + requestContext.ServiceHost.Name);

            var teamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsUri);

            return teamProjectCollection.GetService<IRegistration>();
        }

        public static T ReturnDataFromWarehousePropertyBag<T>(WarehouseDataAccessComponent dac, string propertyBagKey, string scope = null)
        {
            var value = dac.GetProperty(scope, propertyBagKey);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
