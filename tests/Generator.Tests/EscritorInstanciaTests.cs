using Common;
using NSubstitute;

namespace Generator.Tests
{
    public class EscritorInstanciaTests
    {
        private const string DirectorioSalida = "nombreCarpeta";
        private const string ArchivoSalida = "instancia.dat";

        private readonly decimal[,] _instancia = new decimal[,] { { 1 } };

        [Fact]
        public void Constructor_FileSystemNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EscritorInstancia(null));
            Assert.Equal("fileSystem", ex.ParamName);
        }

        [Fact]
        public void EscribirInstancia_InstanciaNull_LanzaArgumentNullException()
        {
            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia();

            var ex = Assert.Throws<ArgumentNullException>(() => escritorInstancia.EscribirInstancia(null, ArchivoSalida));
            Assert.Equal("instancia", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EscribirInstancia_RutaVacia_LanzaArgumentException(string rutaInvalida)
        {
            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia();

            var ex = Assert.Throws<ArgumentException>(() => escritorInstancia.EscribirInstancia(_instancia, rutaInvalida));
            Assert.Contains("ruta no puede estar vacía", ex.Message);
            Assert.Equal("rutaArchivo", ex.ParamName);
        }

        [Fact]
        public void EscribirInstancia_RutaSinDirectorio_NoPreguntaSiExisteNiIntentaCrearDirectorio()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);
            escritorInstancia.EscribirInstancia(_instancia, ArchivoSalida);

            fileSystemHelper.DidNotReceive().DirectoryExists(Arg.Any<string>());
            fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioExistente_NoIntentaCrearlo()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.DirectoryExists(DirectorioSalida).Returns(true);

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);
            escritorInstancia.EscribirInstancia(_instancia, DirectorioSalida + "/" + ArchivoSalida);

            fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioNoExistente_LoCrea()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.DirectoryExists(DirectorioSalida).Returns(false);

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);
            escritorInstancia.EscribirInstancia(_instancia, DirectorioSalida + "/" + ArchivoSalida);

            fileSystemHelper.Received(1).CreateDirectory(DirectorioSalida);
        }

        [Fact]
        public void EscribirInstancia_InstanciaValida_EscribeContenidoCorrecto()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);
            escritorInstancia.EscribirInstancia(new decimal[,] { { 1, 2, 3 }, { 4, 5, 6 } }, ArchivoSalida);

            fileSystemHelper.Received(1).WriteAllLines(ArchivoSalida, Arg.Is<List<string>>(x =>
                x.Count == 3 &&
                x[0] == "2 3" &&
                x[1] == "1\t2\t3" &&
                x[2] == "4\t5\t6"
            ));
        }

        [Fact]
        public void EscribirInstancia_InstanciaVacia_EscribeSoloEncabezado()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);
            escritorInstancia.EscribirInstancia(new decimal[0, 0], ArchivoSalida);

            fileSystemHelper.Received(1).WriteAllLines(ArchivoSalida, Arg.Is<List<string>>(x =>
                x.Count == 1 &&
                x[0] == "0 0"
            ));
        }

        [Fact]
        public void EscribirInstancia_ErrorEscritura_PropagaExcepcion()
        {
            const string mensajeExcepcionInterna = "Error de disco";

            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper
                .When(x => x.WriteAllLines(Arg.Any<string>(), Arg.Any<List<string>>()))
                .Throw(new IOException(mensajeExcepcionInterna));

            EscritorInstancia escritorInstancia = ObtenerEscritorInstancia(fileSystemHelper);

            var ex = Assert.Throws<IOException>(() => escritorInstancia.EscribirInstancia(_instancia, ArchivoSalida));
            Assert.Contains(mensajeExcepcionInterna, ex.Message);
        }

        private EscritorInstancia ObtenerEscritorInstancia()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            var escritor = new EscritorInstancia(fileSystemHelper);
            return escritor;
        }

        private EscritorInstancia ObtenerEscritorInstancia(FileSystemHelper fileSystemHelper)
        {
            var escritor = new EscritorInstancia(fileSystemHelper);
            return escritor;
        }
    }
}
