using Common;
using NSubstitute;

namespace GeneradorInstancia.Tests
{
    public class EscritorInstanciaTests
    {
        private readonly decimal[,] _instancia = new decimal[1, 1] { { 1 } };

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
            Assert.Throws<ArgumentNullException>(() => new EscritorInstancia(null));
        }

        [Fact]
        public void EscribirInstancia_InstanciaNull_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _escritorInstancia.EscribirInstancia(null, "ruta.dat"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EscribirInstancia_RutaVacia_LanzaArgumentException(string rutaInvalida)
        {
            var ex = Assert.Throws<ArgumentException>(() => _escritorInstancia.EscribirInstancia(_instancia, rutaInvalida));
            Assert.StartsWith("La ruta no puede estar vacía", ex.Message);
        }

        [Fact]
        public void EscribirInstancia_RutaSinDirectorio_NoPreguntaSiExisteNiIntentaCrearDirectorio()
        {
            _escritorInstancia.EscribirInstancia(_instancia, "instancia.dat");

            _fileSystemHelper.DidNotReceive().DirectoryExists(Arg.Any<string>());
            _fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioExistente_NoIntentaCrearlo()
        {
            _fileSystemHelper.DirectoryExists("carpeta").Returns(true);

            _escritorInstancia.EscribirInstancia(_instancia, "carpeta/instancia.dat");

            _fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioNoExistente_LoCrea()
        {
            _fileSystemHelper.DirectoryExists("carpeta").Returns(false);

            _escritorInstancia.EscribirInstancia(_instancia, "carpeta/instancia.dat");

            _fileSystemHelper.Received(1).CreateDirectory("carpeta");
        }

        [Fact]
        public void EscribirInstancia_InstanciaValida_EscribeContenidoCorrecto()
        {
            const string ruta = "instancia.dat";

            _escritorInstancia.EscribirInstancia(new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } }, ruta);

            _fileSystemHelper.Received(1).WriteAllLines(ruta, Arg.Is<List<string>>(x =>
                x.Count == 3 &&
                x[0] == "2 3" &&
                x[1] == "1\t2\t3" &&
                x[2] == "4\t5\t6"
            ));
        }

        [Fact]
        public void EscribirInstancia_InstanciaVacia_EscribeSoloEncabezado()
        {
            const string ruta = "vacio.dat";

            _escritorInstancia.EscribirInstancia(new decimal[0, 0], ruta);

            _fileSystemHelper.Received(1).WriteAllLines(ruta, Arg.Is<List<string>>(x =>
                x.Count == 1 &&
                x[0] == "0 0"
            ));
        }

        [Fact]
        public void EscribirInstancia_ErrorEscritura_PropagaExcepcion()
        {
            _fileSystemHelper
                .When(x => x.WriteAllLines(Arg.Any<string>(), Arg.Any<List<string>>()))
                .Throw(new IOException("Error de disco"));

            var ex = Assert.Throws<IOException>(() => _escritorInstancia.EscribirInstancia(_instancia, "instancia.dat"));
            Assert.Equal("Error al escribir la instancia: Error de disco", ex.Message);
        }
    }
}
