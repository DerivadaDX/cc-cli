using Common;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace App.Tests
{
    public class ParametrosGeneracionTests : IDisposable
    {
        public void Dispose()
        {
            FileSystemHelperFactory.SetearHelper(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Constructor_AtomosInvalido_LanzaArgumentOutOfRangeException()
        {
            var excepcion = Record.Exception(() => new ParametrosGeneracion(0, 1, 100, "instancia.dat", false));

            var ex = Assert.IsType<ArgumentOutOfRangeException>(excepcion);
            Assert.Contains("debe ser mayor que cero", ex.Message);
            Assert.Equal("atomos", ex.ParamName);
        }

        [Fact]
        public void Constructor_AgentesInvalido_LanzaArgumentOutOfRangeException()
        {
            var excepcion = Record.Exception(() => new ParametrosGeneracion(1, 0, 100, "instancia.dat", false));

            var ex = Assert.IsType<ArgumentOutOfRangeException>(excepcion);
            Assert.Contains("debe ser mayor que cero", ex.Message);
            Assert.Equal("agentes", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_ValorMaximoInvalido_LanzaArgumentOutOfRangeException(int valorMaximo)
        {
            var excepcion = Record.Exception(() => new ParametrosGeneracion(1, 1, valorMaximo, "instancia.dat", false));

            var ex = Assert.IsType<ArgumentOutOfRangeException>(excepcion);
            Assert.Contains("debe ser mayor que cero", ex.Message);
            Assert.Equal("valorMaximo", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_RutaSalidaVacia_LanzaArgumentException(string rutaSalida)
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(1, 1, 100, rutaSalida, false));
            Assert.StartsWith("La ruta no puede estar vacía", ex.Message);
            Assert.Equal("rutaSalida", ex.ParamName);
        }

        [Fact]
        public void Constructor_RutaSalidaInvalida_LanzaArgumentException()
        {
            var fileSystemHelper = Substitute.For<FileSystemHelper>();
            fileSystemHelper.GetFullPath("¡ruta-inválida!").Throws(new Exception("La ruta es inválida"));
            FileSystemHelperFactory.SetearHelper(fileSystemHelper);

            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(1, 1, 100, "¡ruta-inválida!", false));
            Assert.StartsWith($"Ruta inválida: La ruta es inválida", ex.Message);
            Assert.Equal("rutaSalida", ex.ParamName);
        }

        [Fact]
        public void Constructor_DatosValidos_CreaInstanciaCorrectamente()
        {
            var parametros = new ParametrosGeneracion(5, 3, 100, "instancia.dat", true);

            Assert.Equal(5, parametros.Atomos);
            Assert.Equal(3, parametros.Agentes);
            Assert.Equal(100, parametros.ValorMaximo);
            Assert.Equal("instancia.dat", parametros.RutaSalida);
            Assert.True(parametros.ValoracionesDisjuntas);
        }
    }
}
