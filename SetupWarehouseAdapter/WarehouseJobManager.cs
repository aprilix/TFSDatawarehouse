using System;
using System.Xml;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;

namespace SetupWarehouseAdapter
{
    internal class WarehouseJobManager
    {
        /// <summary>
        /// The data element name used in the job definition.
        /// </summary>
        private const string JobDataElementName = "Data";

        /// <summary>
        /// The data element value used in the job definition identifying this as a Warehouse job.
        /// </summary>
        private const string WarehouseJobDataElementValue = "JobCategories=Warehouse;";

        /// <summary>
        /// The default processing interval is 2 minutes in TFS 2010.  If you would like to have a different run interval
        /// you must use the WarehouseControlWebService ChangeSetting method.  The value set here will be overwritten
        /// the next time the Warehouse is brought offline then back online.
        /// </summary>
        private int WarehouseJobDefaultProcessingInterval = 60;

        private TfsTeamProjectCollection TeamProjectCollection { get; set; }
        private ITeamFoundationJobService CollectionJobService { get; set; }

        /// <summary>
        /// Create a new Warehouse job manager wrapper connected to the Team Project Collection with the given
        /// uri.
        /// </summary>
        /// <param name="tfsCollectionUri">Uri to the team project collection (i.e. http://localhost:8080/tfs/Collection0)</param>
        public WarehouseJobManager(Uri tfsCollectionUri)
        {
            TeamProjectCollection = new TfsTeamProjectCollection(tfsCollectionUri);
            CollectionJobService = TeamProjectCollection.GetService<ITeamFoundationJobService>();
        }

        /// <summary>
        /// Create the custom Warehouse job if one with the same name doesn't already exist.
        /// </summary>
        /// <param name="jobName">Job name.</param>
        /// <param name="jobExtensionName">Job extension.</param>
        public bool CreateJob(string jobName, string jobExtensionName, int minutesInterval = 2)
        {
            TeamFoundationJobDefinition warehouseJob;
            if (TryGetJobByName(jobName, out warehouseJob))
            {
                return false;
            }

            WarehouseJobDefaultProcessingInterval *= minutesInterval;

            warehouseJob = CreateWarehouseJobDefinition(jobName, jobExtensionName);
            CollectionJobService.UpdateJob(warehouseJob);

            return true;
        }

        /// <summary>
        /// Delete the job with the specified name.
        /// </summary>
        /// <param name="jobName">Name of job.</param>
        /// <returns>True if a job was deleted, false otherwise.</returns>
        public bool DeleteJob(string jobName)
        {
            TeamFoundationJobDefinition warehouseJob;
            if (TryGetJobByName(jobName, out warehouseJob))
            {
                CollectionJobService.DeleteJob(warehouseJob);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to find a job definition with a specified name
        /// </summary>
        /// <param name="jobName">Job name.</param>
        /// <param name="jobDefinition">Job definition found, NULL if none found.</param>
        /// <returns>True if a job was found with the name jobName otherwise False.</returns>
        private bool TryGetJobByName(string jobName, out TeamFoundationJobDefinition jobDefinition)
        {
            jobDefinition = null;

            var jobDefinitions = CollectionJobService.QueryJobs();

            foreach (var definition in jobDefinitions)
            {
                if (String.Equals(definition.Name, jobName, StringComparison.InvariantCultureIgnoreCase))
                {
                    jobDefinition = definition;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Create a new job definition for a Warehouse job.
        /// </summary>
        /// <remarks>
        /// This set up the job with the properties required for it to be considered a Warehouse job.
        /// 
        /// TeamFoundationJobDefinition is sealed so you cannot derive a strongly typed warehouse job.
        /// </remarks>
        /// <param name="jobName">Job name.</param>
        /// <param name="jobExtensionName">Extension name (assembly.class)</param>
        /// <returns>New job definition for the Warehouse job.</returns>
        private TeamFoundationJobDefinition CreateWarehouseJobDefinition(string jobName, string jobExtensionName)
        {
            // Add data node identifying this as a Warehouse job.
            var document = new XmlDocument();
            var dataElement = document.CreateElement(JobDataElementName);
            dataElement.InnerText = WarehouseJobDataElementValue;

            // Create a job definition
            var newDefinition = new TeamFoundationJobDefinition(jobName, jobExtensionName, dataElement);

            // Create a default schedule for the job.  
            var schedule = new TeamFoundationJobSchedule(DateTime.UtcNow, WarehouseJobDefaultProcessingInterval);
            newDefinition.Schedule.Add(schedule);

            return newDefinition;
        }
    }
}
