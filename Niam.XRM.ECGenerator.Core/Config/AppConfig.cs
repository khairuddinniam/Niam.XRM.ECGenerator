using System.IO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using NLog;

namespace Niam.XRM.ECGenerator.Core.Config
{
    public class AppConfig : IAppConfig
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public string ConnectionString { get; set; }
        public string Namespace { get; set; }
        public bool GenerateExtensions { get; set; }
        
        public IOrganizationService CreateOrganizationService()
        {
            Logger.Info("Connecting to dynamics crm...");
            var crmClient = new CrmServiceClient(ConnectionString);
            Logger.Info($"Connected to {crmClient.ConnectedOrgFriendlyName}.");
            return crmClient;
        }

        public static IAppConfig Parse(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<AppConfig>(json);
            return config;
        }
    }
}
