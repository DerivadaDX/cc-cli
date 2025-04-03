using Common;
using NSubstitute;

namespace GeneradorInstancia.Tests
{
    public class EscritorInstanciaTests
    {
        [Fact]
        public void Constructor_FileSystemNull_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EscritorInstancia(null));
        }

        [Fact]
        public void EscribirInstancia_InstanciaNull_LanzaArgumentNullException()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            var escritor = new EscritorInstancia(fileSystemHelper);
            Assert.Throws<ArgumentNullException>(() => escritor.EscribirInstancia(null, "ruta.dat"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EscribirInstancia_RutaVacia_LanzaArgumentException(string rutaInvalida)
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            var escritor = new EscritorInstancia(fileSystemHelper);

            var ex = Assert.Throws<ArgumentException>(() => escritor.EscribirInstancia(new decimal[1, 1] { { 1 } }, rutaInvalida));
            Assert.StartsWith("La ruta no puede estar vacía", ex.Message);
        }

        [Fact]
        public void EscribirInstancia_RutaSinDirectorio_NoPreguntaSiExisteNiIntentaCrearDirectorio()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            var escritor = new EscritorInstancia(fileSystemHelper);
            escritor.EscribirInstancia(new decimal[1, 1] { { 1 } }, "instancia.dat");

            fileSystemHelper.DidNotReceive().DirectoryExists(Arg.Any<string>());
            fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioExistente_NoIntentaCrearlo()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.DirectoryExists("carpeta").Returns(true);

            var escritor = new EscritorInstancia(fileSystemHelper);
            escritor.EscribirInstancia(new decimal[1, 1] { { 1 } }, "carpeta/instancia.dat");

            fileSystemHelper.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void EscribirInstancia_DirectorioNoExistente_LoCrea()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.DirectoryExists("carpeta").Returns(false);

            var escritor = new EscritorInstancia(fileSystemHelper);
            escritor.EscribirInstancia(new decimal[1, 1] { { 1 } }, "carpeta/instancia.dat");

            fileSystemHelper.Received(1).CreateDirectory("carpeta");
        }

        [Fact]
        public void EscribirInstancia_InstanciaValida_EscribeContenidoCorrecto()
        {
            const string ruta = "instancia.dat";
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            var escritor = new EscritorInstancia(fileSystemHelper);
            escritor.EscribirInstancia(new decimal[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } }, ruta);

            fileSystemHelper.Received(1).WriteAllLines(ruta, Arg.Is<List<string>>(x =>
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
            var fileSystemHelper = Substitute.For<FileSystemHelper>();

            var escritor = new EscritorInstancia(fileSystemHelper);
            escritor.EscribirInstancia(new decimal[0, 0], ruta);

            fileSystemHelper.Received(1).WriteAllLines(ruta, Arg.Is<List<string>>(x =>
                x.Count == 1 &&
                x[0] == "0 0"
            ));
        }

        [Fact]
        public void EscribirInstancia_ErrorEscritura_PropagaExcepcion()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper
                .When(x => x.WriteAllLines(Arg.Any<string>(), Arg.Any<List<string>>()))
                .Throw(new IOException("Error de disco"));

            var escritor = new EscritorInstancia(fileSystemHelper);

            var ex = Assert.Throws<IOException>(() => escritor.EscribirInstancia(new decimal[1, 1] { { 1 } }, "instancia.dat"));
            Assert.Equal("Error al escribir la instancia: Error de disco", ex.Message);
        }
    }
}
