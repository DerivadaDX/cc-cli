using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorArchivoMatrizValoracionesTests
    {
        private const string RutaArchivo = "rutaArchivo.txt";

        private readonly FileSystemHelper _fileSystemHelper;
        private readonly LectorArchivoMatrizValoraciones _lector;

        public LectorArchivoMatrizValoracionesTests()
        {
            _fileSystemHelper = Substitute.For<FileSystemHelper>();
            _fileSystemHelper.FileExists(Arg.Any<string>()).Returns(true);
            _fileSystemHelper.ReadAllLines(Arg.Any<string>()).Returns([]);

            _lector = new LectorArchivoMatrizValoraciones(_fileSystemHelper);
        }

        [Fact]
        public void Constructor_FileSystemHelperNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LectorArchivoMatrizValoraciones(null));
            Assert.Contains("fileSystemHelper", ex.Message);
        }

        [Fact]
        public void Leer_RutaNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _lector.Leer(null));
            Assert.Contains("rutaArchivo", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("rutaArchivo.txt")]
        public void Leer_ArchivoNoExistente_ArrojaArgumentException(string rutaArchivo)
        {
            _fileSystemHelper.FileExists(rutaArchivo).Returns(false);

            var ex = Assert.Throws<ArgumentException>(() => _lector.Leer(rutaArchivo));
            Assert.StartsWith($"No existe el archivo '{rutaArchivo}'", ex.Message);
        }

        [Fact]
        public void Leer_ArchivoVacio_ArrojaFormatException()
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.Equal("El archivo está vacío o tiene un formato inválido", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("1 2 3")]
        public void Leer_PrimeraLineaConFormatoIncorrecto_ArrojaFormatException(string primeraLinea)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.Equal("La primera línea debe contener las dimensiones en formato '#filas #columnas'", ex.Message);
        }

        [Theory]
        [InlineData("@ 1", "@")]
        [InlineData("Algo 2", "Algo")]
        public void Leer_CantidadFilasInvalida_ArrojaFormatException(string primeraLinea, string valorFilas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.StartsWith($"El valor indicado para filas no es numérico: {valorFilas}", ex.Message);
        }

        [Theory]
        [InlineData("1 @", "@")]
        [InlineData("2 Algo", "Algo")]
        public void Leer_CantidadColumnasInvalida_ArrojaFormatException(string primeraLinea, string valorColumnas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.StartsWith($"El valor indicado para columnas no es numérico: {valorColumnas}", ex.Message);
        }

        [Fact]
        public void Leer_NumeroFilasEsperadasNoCoincideConEncontradas_ArrojaFormatException()
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["1 3", "1\t2\t3", "4\t5\t6"]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.Equal("Filas esperadas: 1, encontradas: 2", ex.Message);
        }

        [Theory]
        [InlineData("", "4\t5\t6", "7\t8\t9", 0, 1)]
        [InlineData("1\t2\t3", "4\t5", "7\t8\t9", 1, 2)]
        [InlineData("1\t2\t3", "4\t5\t6", "7\t8\t9\t0", 2, 4)]
        public void Leer_NumeroColumnasEsperadasNoCoincideConEncontradas_ArrojaFormatException(
            string fila0, string fila1, string fila2, int filaConError, int columnasEncontradas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["3 3", fila0, fila1, fila2]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.StartsWith($"Fila {filaConError}, columnas esperadas: 3, encontradas: {columnasEncontradas}", ex.Message);
        }

        [Theory]
        [InlineData("@\t2\t3", "4\t5\t6", "@", 0, 0)]
        [InlineData("1\t2\t3", "4\t5\tAlgo", "Algo", 1, 2)]
        public void Leer_CeldaConValorInvalido_ArrojaFormatException(
            string fila0, string fila1, string valorInvalido, int filaConError, int columnaConError)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["2 3", fila0, fila1]);

            var ex = Assert.Throws<FormatException>(() => _lector.Leer(RutaArchivo));
            Assert.Equal($"Valor inválido '{valorInvalido}' en ({filaConError}, {columnaConError})", ex.Message);
        }

        [Theory]
        [InlineData("4.4\t5.5\t6.6")]
        [InlineData("   4.4\t5.5\t6.6   ")]
        public void Leer_ArchivoValido_NoArrojaExcepciones(string fila1)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["2 3", "1.1\t2.2\t3.3", fila1]);

            decimal[,] matriz = _lector.Leer(RutaArchivo);

            Assert.Equal(2, matriz.GetLength(0));
            Assert.Equal(3, matriz.GetLength(1));
            Assert.Equal(1.1m, matriz[0, 0]);
            Assert.Equal(2.2m, matriz[0, 1]);
            Assert.Equal(3.3m, matriz[0, 2]);
            Assert.Equal(4.4m, matriz[1, 0]);
            Assert.Equal(5.5m, matriz[1, 1]);
            Assert.Equal(6.6m, matriz[1, 2]);
        }
    }
}