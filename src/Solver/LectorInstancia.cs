using Common;

namespace Solver
{
    internal class LectorInstancia
    {
        private const char SeparadorColumnas = '\t';

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

            (int filas, int columnas) = ObtenerDimensiones(lineas[0]);

            if (filas != lineas.Length - 1)
                throw new FormatException($"Filas esperadas: {filas}, encontradas: {lineas.Length - 1}");

            for (int numeroFila = 1; numeroFila < lineas.Length; numeroFila++)
            {
                string[] valores = lineas[numeroFila].Trim().Split(SeparadorColumnas);
                if (columnas != valores.Length)
                    throw new FormatException($"Fila {numeroFila}, columnas esperadas: {columnas}, encontradas: {valores.Length}");
            }
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
