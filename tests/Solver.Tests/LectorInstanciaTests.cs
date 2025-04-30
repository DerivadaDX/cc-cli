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
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1 2 3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.Equal("La primera línea debe contener las dimensiones en formato '#filas #columnas'", ex.Message);
        }

        [Theory]
        [InlineData("@ 1", "@")]
        [InlineData("Algo 2", "Algo")]
        public void LeerInstancia_CantidadFilasInvalida_ArrojaFormatException(string primeraLinea, string valorFilas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1 2 3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.StartsWith($"El valor indicado para filas no es numérico: {valorFilas}", ex.Message);
        }

        [Theory]
        [InlineData("1 @", "@")]
        [InlineData("2 Algo", "Algo")]
        public void LeerInstancia_CantidadColumnasInvalida_ArrojaFormatException(string primeraLinea, string valorColumnas)
        {
            _fileSystemHelper.ReadAllLines(RutaArchivo).Returns([primeraLinea, "1 2 3"]);

            var ex = Assert.Throws<FormatException>(() => _lectorInstancia.LeerInstancia(RutaArchivo));
            Assert.StartsWith($"El valor indicado para columnas no es numérico: {valorColumnas}", ex.Message);
        }
    }
}