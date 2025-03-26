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

        public void EscribirInstancia(decimal[,] instancia, string rutaArchivo)
        {
            ArgumentNullException.ThrowIfNull(instancia);

            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta no puede estar vacía", nameof(rutaArchivo));

            try
            {
                string? directorio = Path.GetDirectoryName(rutaArchivo);
                if (!string.IsNullOrEmpty(directorio) && !_fileSystem.DirectoryExists(directorio))
                {
                    _fileSystem.CreateDirectory(directorio);
                }

                var lineas = new List<string>
                {
                    $"{instancia.GetLength(0)} {instancia.GetLength(1)}"
                };

                for (int indiceFila = 0; indiceFila < instancia.GetLength(0); indiceFila++)
                {
                    var fila = new decimal[instancia.GetLength(1)];
                    for (int indiceColumna = 0; indiceColumna < instancia.GetLength(1); indiceColumna++)
                    {
                        fila[indiceColumna] = instancia[indiceFila, indiceColumna];
                    }
                    lineas.Add(string.Join("\t", fila));
                }

                _fileSystem.WriteAllLines(rutaArchivo, lineas);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al escribir la instancia: {ex.Message}", ex);
            }
        }
    }
}
