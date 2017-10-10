using Niam.XRM.ECGenerator.Core;
using Niam.XRM.ECGenerator.Core.Config;

namespace Niam.XRM.ECGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = AppConfig.Parse("config.json");
            var generator = config.CreateGenerator();
            var results = generator.Generate();

            foreach (var result in results)
                result.WriteToFile();
        }
    }
}
