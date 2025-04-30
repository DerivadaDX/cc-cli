using Common;

namespace Solver
{
    internal class LectorInstancia
    {
        private FileSystemHelper _fileSystemHelper;

        public LectorInstancia(FileSystemHelper fileSystemHelper)
        {
            ArgumentNullException.ThrowIfNull(fileSystemHelper, nameof(fileSystemHelper));
            _fileSystemHelper = fileSystemHelper;
        }

        internal void LeerInstancia(string rutaArchivo)
        {
            ArgumentNullException.ThrowIfNull(rutaArchivo, nameof(rutaArchivo));
            if (!_fileSystemHelper.FileExists(rutaArchivo))
                throw new ArgumentException($"No existe el archivo '{rutaArchivo}'", nameof(rutaArchivo));
        }
    }
}
