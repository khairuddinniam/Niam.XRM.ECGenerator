using System.IO;

namespace Niam.XRM.ECGenerator.Core
{
    public class EntityOutput : IEntityOutput
    {
        public const string OutputFolder = "result";

        public string FilePath { get; set; }
        public string Content { get; set; }

        public EntityOutput()
        {
        }

        public EntityOutput(string fileName, string content)
        {
            FilePath = Path.Combine(OutputFolder, fileName);
            Content = content;
        }

        public void WriteToFile()
        {
            var dirPath = new FileInfo(FilePath).Directory?.FullName;
            if (dirPath != null && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllText(FilePath, Content);
        }
    }
}