using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NLog;
using Niam.XRM.ECGenerator.Core.Config;

namespace Niam.XRM.ECGenerator.Core
{
    public class CrmGenerator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAppConfig _config;

        public CrmGenerator(IAppConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IEntityOutput[] Generate()
        {
            var metadata = GetAllEntityMetadata();

            var results = new List<IEntityOutput>();
            Logger.Info("Generating entities...");
            results.Add(new EntitiesGenerator(metadata, _config.Namespace).Generate());
            Logger.Info("Entities generated.");

            if (_config.GenerateExtensions)
            {
                Logger.Info("Generating Entities.Extensions...");
                results.Add(new ExtensionsGenerator.ExtensionClassesGenerator(_config, metadata).Generate());
                Logger.Info("Entities.Extensions generated.");
            }

            return results.ToArray();
        }

        public EntityMetadata[] GetAllEntityMetadata()
        {
            var service = _config.CreateOrganizationService();
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Privileges,
                RetrieveAsIfPublished = true
            };

            Logger.Info("Retrieving all entity metadata...");
            var response = (RetrieveAllEntitiesResponse) service.Execute(request);
            Logger.Info("All entity metadata retrieved.");
            return response.EntityMetadata;
        }
    }
}
