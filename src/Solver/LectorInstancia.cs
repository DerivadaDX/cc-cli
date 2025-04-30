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

            string[] lineas = _fileSystemHelper.ReadAllLines(rutaArchivo);
            ValidarFormatoArchivo(lineas);
        }

        private void ValidarFormatoArchivo(string[] lineas)
        {
            if (lineas.Length < 1)
                throw new FormatException("El archivo está vacío o tiene un formato inválido");

            ObtenerDimensiones(lineas[0]);
        }

        private (int filas, int columnas) ObtenerDimensiones(string primeraLinea)
        {
            string[] partes = primeraLinea.Trim().Split(' ');
            if (partes.Length != 2)
                throw new FormatException("La primera línea debe contener las dimensiones en formato '#filas #columnas'");

            bool cantidadfilasInvalida = !int.TryParse(partes[0], out int filas);
            if (cantidadfilasInvalida)
                throw new FormatException($"El valor indicado para filas no es numérico: {partes[0]}");

            bool cantidadcolumnasInvalida = !int.TryParse(partes[1], out int columnas);
            if (cantidadcolumnasInvalida)
                throw new FormatException($"El valor indicado para columnas no es numérico: {partes[1]}");

            return (filas, columnas);
        }
    }
}
