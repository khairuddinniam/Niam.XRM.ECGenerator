using Niam.XRM.ECGenerator.Core.Config;

namespace Niam.XRM.ECGenerator.Core
{
    public static class Extensions
    {
        public static CrmGenerator CreateGenerator(this IAppConfig config)
            => new CrmGenerator(config);
    }
}
