using Microsoft.Xrm.Sdk;

namespace Niam.XRM.ECGenerator.Core.Config
{
    public interface IAppConfig
    {
        string ConnectionString { get; }
        string Namespace { get; }
        bool GenerateExtensions { get; }

        IOrganizationService CreateOrganizationService();
    }
}