using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Warehouse.Adapter.Utils
{
    public class Log
    {
        internal enum LogEventInformationLevel
        {
            Information,
            Warning,
            Error
        }

        internal static void LogEvent(TeamFoundationRequestContext requestContext, string message, LogEventInformationLevel informationLevel)
        {
            switch (informationLevel)
            {
                case LogEventInformationLevel.Information:
                    TeamFoundationTrace.Info(message);
                    break;
                case LogEventInformationLevel.Warning:
                    TeamFoundationTrace.Warning(message);
                    TeamFoundationApplicationCore.Log(requestContext, message, TeamFoundationEventId.WarehouseErrorsBaseEventId, System.Diagnostics.EventLogEntryType.Warning);
                    break;
                case LogEventInformationLevel.Error:
                    TeamFoundationTrace.Error(message);
                    TeamFoundationApplicationCore.Log(requestContext, message, TeamFoundationEventId.WarehouseErrorsBaseEventId, System.Diagnostics.EventLogEntryType.Error);
                    break;
            }
        }
    }
}
