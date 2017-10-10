namespace Niam.XRM.ECGenerator.Core
{
    public interface IEntityOutput
    {
        string FilePath { get; set; }
        string Content { get; }
        void WriteToFile();
    }
}
