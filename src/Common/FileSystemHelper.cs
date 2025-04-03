namespace Common
{
    public class FileSystemHelper
    {
        public virtual void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public virtual void WriteAllLines(string path, IEnumerable<string> lines)
        {
            File.WriteAllLines(path, lines);
        }

        public virtual string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }
    }
}
