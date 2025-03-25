namespace GeneradorInstancias.Tests
{
    public class InstanciaBuilderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeAtomos_CantidadInvalida_LanzaExcepcion(int cantidadAtomos)
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.ConCantidadDeAtomos(cantidadAtomos));
            Assert.StartsWith($"La cantidad de átomos debe ser mayor a cero: {cantidadAtomos}", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeJugadores_CantidadInvalida_LanzaExcepcion(int cantidadJugadores)
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.ConCantidadDeJugadores(cantidadJugadores));
            Assert.StartsWith($"La cantidad de jugadores debe ser mayor a cero: {cantidadJugadores}", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConValorMaximo_CantidadInvalida_LanzaExcepcion(int valorMaximo)
        {
            var builder = new InstanciaBuilder();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.ConValorMaximo(valorMaximo));
            Assert.StartsWith($"El valor máximo debe ser positivo: {valorMaximo}", ex.Message);
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
        public void Build_ValoracionesDisjuntasConMasJugadoresQueAtomos_LanzaExcepcion()
        {
            var builder = new InstanciaBuilder()
                .ConCantidadDeAtomos(2)
                .ConCantidadDeJugadores(3)
                .ConValoracionesDisjuntas(true);
            var ex = Assert.Throws<InvalidOperationException>(builder.Build);
            Assert.Equal("No se puede generar una instancia con más jugadores que átomos si las valoraciones son disjuntas", ex.Message);
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

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void Build_ValorMaximoConfigurado_RespetaLimiteSuperior(int valorMaximo)
        {
            var builder = new InstanciaBuilder()
                .ConCantidadDeAtomos(3)
                .ConCantidadDeJugadores(3)
                .ConValorMaximo(valorMaximo);

            decimal[][] instancia = builder.Build();

            Assert.InRange(instancia[0][0], 0, valorMaximo);
            Assert.InRange(instancia[0][1], 0, valorMaximo);
            Assert.InRange(instancia[0][2], 0, valorMaximo);
            Assert.InRange(instancia[1][0], 0, valorMaximo);
            Assert.InRange(instancia[1][1], 0, valorMaximo);
            Assert.InRange(instancia[1][2], 0, valorMaximo);
            Assert.InRange(instancia[2][0], 0, valorMaximo);
            Assert.InRange(instancia[2][1], 0, valorMaximo);
            Assert.InRange(instancia[2][2], 0, valorMaximo);
        }

        [Fact]
        public void Build_ValoracionesNoDisjuntas_NingunaCeldaValeCero()
        {
            var builder = new InstanciaBuilder()
                .ConCantidadDeAtomos(3)
                .ConCantidadDeJugadores(3)
                .ConValorMaximo(1)
                .ConValoracionesDisjuntas(false);

            decimal[][] instancia = builder.Build();

            Assert.NotEqual(0, instancia[0][0]);
            Assert.NotEqual(0, instancia[0][1]);
            Assert.NotEqual(0, instancia[0][2]);
            Assert.NotEqual(0, instancia[1][0]);
            Assert.NotEqual(0, instancia[1][1]);
            Assert.NotEqual(0, instancia[1][2]);
            Assert.NotEqual(0, instancia[2][0]);
            Assert.NotEqual(0, instancia[2][1]);
            Assert.NotEqual(0, instancia[2][2]);
        }

        [Fact]
        public void Build_ValoracionesDisjuntas_CadaFilaTieneUnSoloValorPositivo()
        {
            var builder = new InstanciaBuilder()
                .ConCantidadDeAtomos(4)
                .ConCantidadDeJugadores(3)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true);

            decimal[][] instancia = builder.Build();

            Assert.Equal(1, instancia[0].Count(v => v > 0));
            Assert.Equal(1, instancia[1].Count(v => v > 0));
            Assert.Equal(1, instancia[2].Count(v => v > 0));
            Assert.Equal(1, instancia[3].Count(v => v > 0));
        }
    }
}