using Common;

namespace Solver
{
    public class LectorArchivoMatrizValoraciones
    {
        private const char SeparadorColumnas = '\t';

        private readonly FileSystemHelper _fileSystemHelper;

        public LectorArchivoMatrizValoraciones(FileSystemHelper fileSystemHelper)
        {
            ArgumentNullException.ThrowIfNull(fileSystemHelper, nameof(fileSystemHelper));
            _fileSystemHelper = fileSystemHelper;
        }

        public decimal[,] Leer(string rutaArchivo)
        {
            ArgumentNullException.ThrowIfNull(rutaArchivo, nameof(rutaArchivo));
            if (!_fileSystemHelper.FileExists(rutaArchivo))
                throw new ArgumentException($"No existe el archivo '{rutaArchivo}'", nameof(rutaArchivo));

            string[] lineas = _fileSystemHelper.ReadAllLines(rutaArchivo);
            ValidarFormatoArchivo(lineas);

            decimal[,] matriz = ParsearLineasAMatriz(lineas);
            return matriz;
        }

        private void ValidarFormatoArchivo(string[] lineas)
        {
            if (lineas.Length < 1)
                throw new FormatException("El archivo está vacío o tiene un formato inválido");

            (int filas, int columnas) = ObtenerDimensionesMatriz(lineas[0]);

            if (filas != lineas.Length - 1)
                throw new FormatException($"Filas esperadas: {filas}, encontradas: {lineas.Length - 1}");

            for (int indiceFila = 0; indiceFila < filas; indiceFila++)
            {
                string[] valores = lineas[indiceFila + 1].Trim().Split(SeparadorColumnas);
                if (columnas != valores.Length)
                    throw new FormatException($"Fila {indiceFila}, columnas esperadas: {columnas}, encontradas: {valores.Length}");
            }
        }

        private decimal[,] ParsearLineasAMatriz(string[] lineas)
        {
            (int filas, int columnas) = ObtenerDimensionesMatriz(lineas[0]);
            var matriz = new decimal[filas, columnas];

            for (int indiceFila = 0; indiceFila < filas; indiceFila++)
            {
                string[] valores = lineas[indiceFila + 1].Trim().Split(SeparadorColumnas);
                for (int indiceColumna = 0; indiceColumna < columnas; indiceColumna++)
                {
                    bool valorInvalido = !decimal.TryParse(valores[indiceColumna], out decimal valor);
                    if (valorInvalido)
                        throw new FormatException($"Valor inválido '{valores[indiceColumna]}' en ({indiceFila}, {indiceColumna})");

                    matriz[indiceFila, indiceColumna] = valor;
                }
            }

            return matriz;
        }

        private (int filas, int columnas) ObtenerDimensionesMatriz(string primeraLinea)
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
