using System;
using System.Globalization;
using SetupWarehouseAdapter.Properties;

namespace SetupWarehouseAdapter
{
    /// <summary>
    /// A simple command line tool used to create or delete a Warehouse job and schedule for the CSharp Assembly Code Churn adapter.
    /// 
    /// In order to fully install and configure this custom adapter you need to do the following:
    ///  - Build the CSharpAssemblyCodeChurnSample.Adapter and copy it to the 
    ///      %ProgramFiles%\Microsoft Team Foundation Server 2010\Application Tier\TFSJobAgent\plugins
    ///    directory on all of your Team Foundation Server Application Tier machines.  Assemblies in
    ///    this location will be loaded by the TFS Job Agent.
    ///  - Restart the "Visual Studio Team Foundation Background Job Agent" service.  This will
    ///    force the TFS Job Agent to reload all the job plugins.
    ///      net stop TfsJobAgent
    ///      net start TfsJobAgent
    ///  - Run this executable with the -c flag.  This will create a TFS Job and schedule for the custom
    ///    adapter.
    /// 
    /// In order to fully uninstall this custom adapter and remove all related schema from the Warehouse 
    /// you will need to do the following:
    ///  - Run this executable with the -d flag.  This will delete the TFS Job and schedule for the custom
    ///    adapter.
    ///  - Delete the CSharpAssemblyCodeChurnSample.Adapter assembly from the 
    ///      %ProgramFiles%\Microsoft Team Foundation Server 2010\Application Tier\TFSJobAgent\plugins
    ///    directory.
    ///  - Rebuild the Warehouse
    ///      Either use TfsConfig.exe rebuildWarehouse /all
    ///              or Tfs Administration Console, under the Application Tier -> Reporting pane click "Start Rebuild"
    ///    This will rebuild the Warehouse schema and data without the custom schema added by this adapter.
    /// </summary>
    public class ManageCustomWarehouseJob
    {
        static void Main(string[] args)
        {
            var jobName = "";
            var syncJobExtension = "";
            var collection = "";
            var create = false;
            var delete = false;
            var minutesInterval = 2;

            if (args.GetLength(0) == 0)
            {
                Console.WriteLine(Resources.Usage);
                return;
            }

            foreach (var arg in args)
            {
                var nomeArgumento = arg.ToLower();

                if (nomeArgumento.Contains("="))
                    nomeArgumento = nomeArgumento.Split('=')[0];

                if (nomeArgumento == "-jobname")
                {
                    jobName = arg.Split('=')[1];
                }
                else if (nomeArgumento == "-syncjobextension")
                {
                    syncJobExtension = arg.Split('=')[1];
                }
                else if (nomeArgumento == "-collection")
                {
                    collection = arg.Split('=')[1];
                }
                else if (nomeArgumento == "-c")
                {
                    create = true;
                }
                else if (nomeArgumento == "-d")
                {
                    delete = true;
                }
                else if (nomeArgumento == "-t")
                {
                    minutesInterval = int.Parse(arg.Split('=')[1]);
                }
                else
                {
                    Console.WriteLine(Resources.Usage);
                    return;
                }
            }

            var jobManager = new WarehouseJobManager(new Uri(collection));

            if (create)
            {
                if (jobManager.CreateJob(jobName, syncJobExtension, minutesInterval))
                {
                    Console.WriteLine(String.Format(CultureInfo.CurrentCulture,
                        Resources.CreatedJob,
                        jobName, syncJobExtension));
                }
                else
                {
                    Console.WriteLine(String.Format(CultureInfo.CurrentCulture,
                        Resources.JobAlreadyExists, jobName));
                }
            }
            else if (delete)
            {
                if (jobManager.DeleteJob(jobName))
                {
                    Console.WriteLine(String.Format(CultureInfo.CurrentCulture,
                        Resources.JobDeleted, jobName));
                }
                else
                {
                    Console.WriteLine(String.Format(CultureInfo.CurrentCulture,
                        Resources.JobNotFound, jobName));
                }
            }
            else
            {
                Console.WriteLine(Resources.Usage);
                return;
            }
        }
    }
}
