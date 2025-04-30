using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorInstanciaTests
    {
        private const string RutaArchivo = "rutaArchivo.txt";

        private readonly FileSystemHelper _fileSystemHelper;
        private readonly LectorInstancia _lectorInstancia;

        public LectorInstanciaTests()
        {
            _fileSystemHelper = Substitute.For<FileSystemHelper>();
            _fileSystemHelper.FileExists(Arg.Any<string>()).Returns(true);
            _fileSystemHelper.ReadAllLines(Arg.Any<string>()).Returns([]);

            _lectorInstancia = new LectorInstancia(_fileSystemHelper);
        }

        [Fact]
        public void Constructor_FileSystemHelperNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LectorInstancia(null));
            Assert.Contains("fileSystemHelper", ex.Message);
        }

        [Fact]
        public void LeerInstancia_RutaNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _lectorInstancia.LeerInstancia(null));
            Assert.Contains("rutaArchivo", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("rutaArchivo.txt")]
        public void LeerInstancia_ArchivoNoExistente_ArrojaArgumentException(string rutaArchivo)
        {
            _fileSystemHelper.FileExists(rutaArchivo).Returns(false);

            var ex = Assert.Throws<ArgumentException>(() => _lectorInstancia.LeerInstancia(rutaArchivo));
            Assert.StartsWith($"No existe el archivo '{rutaArchivo}'", ex.Message);
        }

        [Fact]
        public void LeerInstancia_ArchivoVacio_ArrojaFormatException()
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.Equal("El archivo está vacío o tiene un formato inválido", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("1 2 3")]
        public void LeerInstancia_PrimeraLineaConFormatoIncorrecto_ArrojaFormatException(string primeraLinea)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.Equal("La primera línea debe contener las dimensiones en formato '#filas #columnas'", ex.Message);
        }

        [Theory]
        [InlineData("@ 1", "@")]
        [InlineData("Algo 2", "Algo")]
        public void LeerInstancia_CantidadFilasInvalida_ArrojaFormatException(string primeraLinea, string valorFilas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.StartsWith($"El valor indicado para filas no es numérico: {valorFilas}", ex.Message);
        }

        [Theory]
        [InlineData("1 @", "@")]
        [InlineData("2 Algo", "Algo")]
        public void LeerInstancia_CantidadColumnasInvalida_ArrojaFormatException(string primeraLinea, string valorColumnas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1\t2\t3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.StartsWith($"El valor indicado para columnas no es numérico: {valorColumnas}", ex.Message);
        }

        [Fact]
        public void LeerInstancia_NumeroFilasEsperadasNoCoincideConEncontradas_ArrojaFormatException()
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["1 3", "1\t2\t3", "4\t5\t6"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.Equal("Filas esperadas: 1, encontradas: 2", ex.Message);
        }

        [Theory]
        [InlineData("", "4\t5\t6", "7\t8\t9", 0, 1)]
        [InlineData("1\t2\t3", "4\t5", "7\t8\t9", 1, 2)]
        [InlineData("1\t2\t3", "4\t5\t6", "7\t8\t9\t0", 2, 4)]
        public void LeerInstancia_NumeroColumnasEsperadasNoCoincideConEncontradas_ArrojaFormatException(
            string fila0, string fila1, string fila2, int filaConError, int columnasEncontradas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["3 3", fila0, fila1, fila2]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.StartsWith($"Fila {filaConError}, columnas esperadas: 3, encontradas: {columnasEncontradas}", ex.Message);
        }

        [Theory]
        [InlineData("@\t2\t3", "4\t5\t6", "@", 0, 0)]
        [InlineData("1\t2\t3", "4\t5\tAlgo", "Algo", 1, 2)]
        public void LeerInstancia_CeldaConValorInvalido_ArrojaFormatException(
            string fila0, string fila1, string valorInvalido, int filaConError, int columnaConError)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns(["2 3", fila0, fila1]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.Equal($"Valor inválido '{valorInvalido}' en ({filaConError}, {columnaConError})", ex.Message);
        }

        [Theory]
        [InlineData("4.4\t5.5\t6.6")]
        [InlineData("   4.4\t5.5\t6.6   ")]
        public void LeerInstancia_ArchivoValido_NoArrojaExcepciones(string segundaLinea)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([
                "2 3",
                "1.1\t2.2\t3.3",
                segundaLinea
            ]);

            decimal[,] instancia = _lectorInstancia.LeerInstancia(RutaArchivo);
            Assert.Equal(2, instancia.GetLength(0));
            Assert.Equal(3, instancia.GetLength(1));
            Assert.Equal(1.1m, instancia[0, 0]);
            Assert.Equal(2.2m, instancia[0, 1]);
            Assert.Equal(3.3m, instancia[0, 2]);
            Assert.Equal(4.4m, instancia[1, 0]);
            Assert.Equal(5.5m, instancia[1, 1]);
            Assert.Equal(6.6m, instancia[1, 2]);
        }
    }
}