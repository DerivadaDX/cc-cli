using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorInstanciaTests
    {
        [Fact]
        public void Constructor_FileSystemHelperNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LectorInstancia(null));
            Assert.Contains("fileSystemHelper", ex.Message);
        }

        [Fact]
        public void LeerInstancia_RutaNull_ArrojaArgumentNullException()
        {
            var lectorInstancia = new LectorInstancia(Substitute.For<FileSystemHelper>());
            var ex = Assert.Throws<ArgumentNullException>(() => lectorInstancia.LeerInstancia(null));
            Assert.Contains("rutaArchivo", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("rutaArchivo.txt")]
        public void LeerInstancia_ArchivoNoExistente_ArrojaArgumentException(string rutaArchivo)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.FileExists(Arg.Any<string>()).Returns(false);

            var lectorInstancia = new LectorInstancia(Substitute.For<FileSystemHelper>());

            var ex = Assert.Throws<ArgumentException>(() => lectorInstancia.LeerInstancia(rutaArchivo));
            Assert.StartsWith($"No existe el archivo '{rutaArchivo}'", ex.Message);
        }
    }
}
