namespace GeneradorInstancias.Tests
{
    public class InstanciaBuilderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCAntidadDeAtomos_CantidadInvalida_LanzaExcepcion(int cantidadAtomos)
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.ConCantidadDeAtomos(cantidadAtomos));
            Assert.StartsWith($"La cantidad de átomos debe ser mayor a cero: {cantidadAtomos}", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCAntidadDeJugadores_CantidadInvalida_LanzaExcepcion(int cantidadJugadores)
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.ConCantidadDeJugadores(cantidadJugadores));
            Assert.StartsWith($"La cantidad de jugadores debe ser mayor a cero: {cantidadJugadores}", ex.Message);
        }

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
            var builder = new InstanciaBuilder().ConCantidadDeAtomos(2);
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("Debe especificar el número de átomos y jugadores antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_SoloJugadoresConfigurados_LanzaExcepcion()
        {
            var builder = new InstanciaBuilder().ConCantidadDeJugadores(2);
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("Debe especificar el número de átomos y jugadores antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_AtomosYJugadoresValidos_DevuelveMatrizCorrecta()
        {
            var builder = new InstanciaBuilder()
                .ConCantidadDeAtomos(3)
                .ConCantidadDeJugadores(2);

            decimal[][] instancia = builder.Build();

            Assert.Equal(3, instancia.Length);
            Assert.Equal(2, instancia[0].Length);
        }
    }
}