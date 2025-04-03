namespace Common
{
    public interface IFileSystemHelper
    {
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
        void WriteAllLines(string path, IEnumerable<string> lines);
    }
}