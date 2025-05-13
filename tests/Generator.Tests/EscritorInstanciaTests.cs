using Common;
using NSubstitute;

namespace Generator.Tests
{
    public class EscritorInstanciaTests
    {
        private const string DirectorioSalida = "nombreCarpeta";
        private const string NombreArchivoSalida = "instancia.dat";

        private readonly decimal[,] _instancia = new decimal[,] { { 1 } };

        private readonly FileSystemHelper _fileSystemHelper;
        private readonly EscritorInstancia _escritorInstancia;

        public EscritorInstanciaTests()
        {
            _fileSystemHelper = Substitute.For<FileSystemHelper>();
            _escritorInstancia = new EscritorInstancia(_fileSystemHelper);
        }

        [Fact]
        public void Constructor_FileSystemNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EscritorInstancia(null));
            Assert.Equal("fileSystem", ex.ParamName);
        }

        [Fact]
        public void EscribirInstancia_InstanciaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                _escritorInstancia.EscribirInstancia(null, NombreArchivoSalida));

            Assert.Equal("instancia", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EscribirInstancia_RutaVacia_LanzaArgumentException(string rutaInvalida)
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                _escritorInstancia.EscribirInstancia(_instancia, rutaInvalida));

            Assert.Contains("ruta no puede estar vacía", ex.Message);
            Assert.Equal("rutaArchivo", ex.ParamName);
        }

        [Fact]
        public void EscribirInstancia_RutaSinDirectorio_NoPreguntaSiExisteNiIntentaCrearDirectorio()
        {
            _escritorInstancia.EscribirInstancia(_instancia, NombreArchivoSalida);

            _fileSystemHelper.DidNotReceive().DirectoryExists(Arg.Any<string>());
            _fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioExistente_NoIntentaCrearlo()
        {
            _fileSystemHelper.DirectoryExists(DirectorioSalida).Returns(true);

            _escritorInstancia.EscribirInstancia(_instancia, DirectorioSalida + "/" + NombreArchivoSalida);

            _fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioNoExistente_LoCrea()
        {
            _fileSystemHelper.DirectoryExists(DirectorioSalida).Returns(false);

            _escritorInstancia.EscribirInstancia(_instancia, DirectorioSalida + "/" + NombreArchivoSalida);

            _fileSystemHelper.Received(1).CreateDirectory(DirectorioSalida);
        }

        [Fact]
        public void EscribirInstancia_InstanciaValida_EscribeContenidoCorrecto()
        {
            _escritorInstancia.EscribirInstancia(new decimal[,] { { 1, 2, 3 }, { 4, 5, 6 } }, NombreArchivoSalida);

            _fileSystemHelper.Received(1).WriteAllLines(NombreArchivoSalida, Arg.Is<List<string>>(x =>
                x.Count == 3 &&
                x[0] == "2 3" &&
                x[1] == "1\t2\t3" &&
                x[2] == "4\t5\t6"
            ));
        }

        [Fact]
        public void EscribirInstancia_InstanciaVacia_EscribeSoloEncabezado()
        {
            _escritorInstancia.EscribirInstancia(new decimal[0, 0], NombreArchivoSalida);

            _fileSystemHelper.Received(1).WriteAllLines(NombreArchivoSalida, Arg.Is<List<string>>(x =>
                x.Count == 1 &&
                x[0] == "0 0"
            ));
        }

        [Fact]
        public void EscribirInstancia_ErrorEscritura_PropagaExcepcion()
        {
            const string mensajeExcepcionInterna = "Error de disco";

            _fileSystemHelper
                .When(x => x.WriteAllLines(Arg.Any<string>(), Arg.Any<List<string>>()))
                .Throw(new IOException(mensajeExcepcionInterna));

            var ex = Assert.Throws<IOException>(() =>
                _escritorInstancia.EscribirInstancia(_instancia, NombreArchivoSalida));

            Assert.Contains(mensajeExcepcionInterna, ex.Message);
        }
    }
}
