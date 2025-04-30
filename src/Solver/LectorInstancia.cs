using Common;

namespace Solver
{
    internal class LectorInstancia
    {
        public LectorInstancia(FileSystemHelper fileSystemHelper)
        {
            ArgumentNullException.ThrowIfNull(fileSystemHelper, nameof(fileSystemHelper));
        }
    }
}
