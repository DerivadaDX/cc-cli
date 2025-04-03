using Common;

namespace GeneradorInstancia
{
    public class EscritorInstancia
    {
        private readonly IFileSystemHelper _fileSystem;

        public EscritorInstancia(IFileSystemHelper fileSystem)
        {
            ArgumentNullException.ThrowIfNull(fileSystem);
            _fileSystem = fileSystem;
        }

        public virtual void EscribirInstancia(decimal[,] instancia, string rutaArchivo)
        {
            ArgumentNullException.ThrowIfNull(instancia);
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta no puede estar vacía", nameof(rutaArchivo));

            try
            {
                CrearDirectorioSiNoExiste(rutaArchivo);

                List<string> lineas = GenerarLineasDeInstancia(instancia);
                _fileSystem.WriteAllLines(rutaArchivo, lineas);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al escribir la instancia: {ex.Message}", ex);
            }
        }

        private void CrearDirectorioSiNoExiste(string rutaArchivo)
        {
            string directorio = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !_fileSystem.DirectoryExists(directorio))
            {
                _fileSystem.CreateDirectory(directorio);
            }
        }

        private List<string> GenerarLineasDeInstancia(decimal[,] instancia)
        {
            List<string> lineas = [$"{instancia.GetLength(0)} {instancia.GetLength(1)}"];

            for (int indiceFila = 0; indiceFila < instancia.GetLength(0); indiceFila++)
            {
                string lineaFila = ObtenerLineaFila(instancia, indiceFila);
                lineas.Add(lineaFila);
            }

            return lineas;
        }

        private string ObtenerLineaFila(decimal[,] instancia, int indiceFila)
        {
            var fila = new decimal[instancia.GetLength(1)];
            for (int indiceColumna = 0; indiceColumna < instancia.GetLength(1); indiceColumna++)
            {
                fila[indiceColumna] = instancia[indiceFila, indiceColumna];
            }

            string lineaFila = string.Join("\t", fila);
            return lineaFila;
        }
    }
}