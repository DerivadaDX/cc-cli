using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorInstanciaTests
    {
        private readonly FileSystemHelper _fileSystemHelper;
        private readonly LectorInstancia _lectorInstancia;

        public LectorInstanciaTests()
        {
            _fileSystemHelper = Substitute.For<FileSystemHelper>();
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
            _fileSystemHelper.FileExists(Arg.Any<string>()).Returns(false);

            var ex = Assert.Throws<ArgumentException>(() => _lectorInstancia.LeerInstancia(rutaArchivo));
            Assert.StartsWith($"No existe el archivo '{rutaArchivo}'", ex.Message);
        }
    }
}
