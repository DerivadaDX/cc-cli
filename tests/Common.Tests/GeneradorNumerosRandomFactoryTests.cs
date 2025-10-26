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
            var generador = GeneradorNumerosRandomFactory.Crear(123);
            Assert.NotNull(generador);
            Assert.IsType<GeneradorNumerosRandom>(generador);
        }

        [Fact]
        public void Crear_SinGeneradorSeteado_DevuelveNuevaInstanciaConSeedSeteada()
        {
            int seed = 456;
            var generador1 = GeneradorNumerosRandomFactory.Crear(seed);
            var generador2 = GeneradorNumerosRandomFactory.Crear(seed);

            Assert.Equal(generador1.Siguiente(100), generador2.Siguiente(100));
        }

        [Fact]
        public void Crear_ConGeneradorSeteado_DevuelveGeneradorSeteado()
        {
            var generadorSeteado = new GeneradorNumerosRandom(123);
            GeneradorNumerosRandomFactory.SetearGenerador(generadorSeteado);

            var generadorObtenido = GeneradorNumerosRandomFactory.Crear(456);

            Assert.Same(generadorSeteado, generadorObtenido);
        }
    }
}
