namespace App.Tests
{
    public class ParametrosGeneracionTests
    {
        [Fact]
        public void Constructor_AtomosInvalido_LanzaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(0, 1, 100, "ruta.txt", false));
            Assert.StartsWith("Se requieren al menos 1 átomo", ex.Message);
        }

        [Fact]
        public void Constructor_AgentesInvalido_LanzaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(1, 0, 100, "ruta.txt", false));
            Assert.StartsWith("Se requieren al menos 1 agente", ex.Message);
        }

        [Fact]
        public void Constructor_ValorMaximoInvalido_LanzaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(1, 1, -1, "ruta.txt", false));
            Assert.StartsWith("El valor máximo no puede ser negativo", ex.Message);
        }

        [Fact]
        public void Constructor_RutaSalidaVacia_LanzaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametrosGeneracion(1, 1, 100, "", false));
            Assert.StartsWith("La ruta no puede estar vacía", ex.Message);
        }

        [Fact]
        public void Constructor_DatosValidos_CreaInstanciaCorrectamente()
        {
            var parametros = new ParametrosGeneracion(5, 3, 100, "test.txt", true);

            Assert.Equal(5, parametros.Atomos);
            Assert.Equal(3, parametros.Agentes);
            Assert.Equal(100, parametros.ValorMaximo);
            Assert.Equal("test.txt", parametros.RutaSalida);
            Assert.True(parametros.ValoracionesDisjuntas);
        }
    }
}
