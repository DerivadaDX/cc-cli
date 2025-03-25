namespace Common.Tests
{
    public class GeneradorNumerosRandomFactoryTests : IDisposable
    {
        public void Dispose()
        {
            GeneradorNumerosRandomFactory.SetearGenerador(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            var generador = GeneradorNumerosRandomFactory.Crear();
            Assert.NotNull(generador);
            Assert.IsType<GeneradorNumerosRandom>(generador);
        }

        [Fact]
        public void Crear_ConSemillaSinGeneradorSeteador_DevuelveNuevaInstanciaConSemilla()
        {
            int semilla = 456;
            var generador = GeneradorNumerosRandomFactory.Crear(semilla);
            var generador2 = GeneradorNumerosRandomFactory.Crear(semilla);

            Assert.Equal(generador.Siguiente(), generador2.Siguiente());
        }

        [Fact]
        public void Crear_ConSemillaConGeneradorSeteador_DevuelveGeneradorSeteado()
        {
            var generadorMock = new GeneradorNumerosRandom(123);
            GeneradorNumerosRandomFactory.SetearGenerador(generadorMock);

            var generador = GeneradorNumerosRandomFactory.Crear(456);

            Assert.Same(generadorMock, generador);
        }

        [Fact]
        public void SetearGenerador_Generador_SeSeteaCorrectamente()
        {
            var generadorMock = new GeneradorNumerosRandom(123);
            GeneradorNumerosRandomFactory.SetearGenerador(generadorMock);

            var generador = GeneradorNumerosRandomFactory.Crear();

            Assert.Same(generadorMock, generador);
        }
    }
}