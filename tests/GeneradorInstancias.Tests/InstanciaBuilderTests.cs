namespace GeneradorInstancias.Tests
{
    public class InstanciaBuilderTests
    {
        [Fact]
        public void Build_SinConfiguracionPrevia_LanzaExcepcion()
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("Debe especificar el número de átomos y jugadores antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_SoloAtomosConfigurados_LanzaExcepcion()
        {
            var builder = new InstanciaBuilder().ConAtomos(2);
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("Debe especificar el número de átomos y jugadores antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_SoloJugadoresConfigurados_LanzaExcepcion()
        {
            var builder = new InstanciaBuilder().ConJugadores(2);
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("Debe especificar el número de átomos y jugadores antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_AtomosYJugadoresValidos_DevuelveMatrizCorrecta()
        {
            var builder = new InstanciaBuilder()
                .ConAtomos(3)
                .ConJugadores(2);

            decimal[][] instancia = builder.Build();

            Assert.Equal(3, instancia.Length);
            Assert.Equal(2, instancia[0].Length);
        }
    }
}