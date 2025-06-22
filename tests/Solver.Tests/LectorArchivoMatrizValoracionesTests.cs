using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorArchivoMatrizValoracionesTests
    {
        private const string RutaArchivo = "rutaArchivo.txt";

        [Fact]
        public void ConstructorfileSystemHelperNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LectorArchivoMatrizValoraciones(null));
            Assert.Equal("fileSystemHelper", ex.ParamName);
        }

        [Fact]
        public void Leer_RutaNull_LanzaArgumentNullException()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<ArgumentNullException>(() => lector.Leer(null));
            Assert.Equal("rutaArchivo", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("rutaArchivo.txt")]
        public void Leer_ArchivoNoExistente_LanzaArgumentException(string rutaArchivo)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(rutaArchivo).Returns(false);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<ArgumentException>(() => lector.Leer(rutaArchivo));
            Assert.Contains("No existe el archivo", ex.Message);
            Assert.Equal("rutaArchivo", ex.ParamName);
        }

        [Fact]
        public void Leer_ArchivoVacio_LanzaFormatException()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("está vacío o tiene un formato inválido", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("1 2 3")]
        public void Leer_PrimeraLineaConFormatoIncorrecto_LanzaFormatException(string primeraLinea)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("debe contener las dimensiones en formato '#filas #columnas'", ex.Message);
        }

        [Theory]
        [InlineData("@ 1")]
        [InlineData("1.1 1")]
        [InlineData("Algo 2")]
        public void Leer_CantidadFilasInvalida_LanzaFormatException(string primeraLinea)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("no es numérico", ex.Message);
        }

        [Theory]
        [InlineData("0 2")]
        [InlineData("-1 2")]
        public void Leer_CantidadFilasNoPositiva_LanzaFormatException(string primeraLinea)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("mayor a cero", ex.Message);
        }

        [Theory]
        [InlineData("1 @")]
        [InlineData("2 Algo")]
        public void Leer_CantidadColumnasInvalida_LanzaFormatException(string primeraLinea)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("no es numérico", ex.Message);
        }

        [Theory]
        [InlineData("2 0")]
        [InlineData("2 -1")]
        public void Leer_CantidadColumnasNoPositiva_LanzaFormatException(string primeraLinea)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("mayor a cero", ex.Message);
        }

        [Fact]
        public void Leer_NumeroFilasEsperadasNoCoincideConEncontradas_LanzaFormatException()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["1 3", "1\t2\t3", "4\t5\t6"]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("Filas esperadas", ex.Message);
            Assert.Contains("encontradas", ex.Message);
        }

        [Theory]
        [InlineData("", "4\t5\t6", "7\t8\t9", 0, 1)]
        [InlineData("1\t2\t3", "4\t5", "7\t8\t9", 1, 2)]
        [InlineData("1\t2\t3", "4\t5\t6", "7\t8\t9\t0", 2, 4)]
        public void Leer_NumeroColumnasEsperadasNoCoincideConEncontradas_LanzaFormatException(
            string fila0, string fila1, string fila2, int filaConError, int columnasEncontradas)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["3 3", fila0, fila1, fila2]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains($"Fila {filaConError}", ex.Message);
            Assert.Contains("columnas esperadas", ex.Message);
            Assert.Contains($"encontradas: {columnasEncontradas}", ex.Message);
        }

        [Theory]
        [InlineData("@\t2\t3", "4\t5\t6")]
        [InlineData("1\t2\t3", "4\t5\tAlgo")]
        public void Leer_CeldaConValorInvalido_LanzaFormatException(string fila0, string fila1)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["2 3", fila0, fila1]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);

            var ex = Assert.Throws<FormatException>(() => lector.Leer(RutaArchivo));
            Assert.Contains("Valor inválido", ex.Message);
        }

        [Theory]
        [InlineData("4.4\t5.5\t6.6")]
        [InlineData("   4.4\t5.5\t6.6   ")]
        public void Leer_ArchivoValido_NoLanzaExcepciones(string fila1)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(RutaArchivo).Returns(true);
            fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["2 3", "1.1\t2.2\t3.3", fila1]);

            LectorArchivoMatrizValoraciones lector = ObtenerLectorArchivoMatrizValoraciones(fileSystemHelper);
            decimal[,] matriz = lector.Leer(RutaArchivo);

            Assert.Equal(2, matriz.GetLength(0));
            Assert.Equal(3, matriz.GetLength(1));
            Assert.Equal(1.1m, matriz[0, 0]);
            Assert.Equal(2.2m, matriz[0, 1]);
            Assert.Equal(3.3m, matriz[0, 2]);
            Assert.Equal(4.4m, matriz[1, 0]);
            Assert.Equal(5.5m, matriz[1, 1]);
            Assert.Equal(6.6m, matriz[1, 2]);
        }

        private LectorArchivoMatrizValoraciones ObtenerLectorArchivoMatrizValoraciones(FileSystemHelper fileSystemHelper)
        {
            var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);
            return lector;
        }
    }
}