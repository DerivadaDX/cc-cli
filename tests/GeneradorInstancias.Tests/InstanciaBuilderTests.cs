namespace GeneradorInstancia.Tests
{
    public class InstanciaBuilderTests
    {
        private readonly InstanciaBuilder _instanciaBuilder;

        public InstanciaBuilderTests()
        {
            _instanciaBuilder = new InstanciaBuilder();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeAtomos_CantidadInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConCantidadDeAtomos(cantidadAtomos));
            Assert.StartsWith($"La cantidad de átomos debe ser mayor a cero: {cantidadAtomos}", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeAgentes_CantidadInvalida_LanzaArgumentOutOfRangeException(int cantidadAgentes)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConCantidadDeAgentes(cantidadAgentes));
            Assert.StartsWith($"La cantidad de agentes debe ser mayor a cero: {cantidadAgentes}", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConValorMaximo_CantidadInvalida_LanzaArgumentOutOfRangeException(int valorMaximo)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConValorMaximo(valorMaximo));
            Assert.StartsWith($"El valor máximo debe ser positivo: {valorMaximo}", ex.Message);
        }

        [Fact]
        public void Build_SinAtomosSeteados_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAgentes(2)
                .Build);

            Assert.Equal("Debe especificar el número de átomos antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_SinAgentesSeteados_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .Build);

            Assert.Equal("Debe especificar el número de agentes antes de construir la instancia", ex.Message);
        }

        [Fact]
        public void Build_ValoracionesDisjuntasConMasAgentesQueAtomos_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .ConCantidadDeAgentes(3)
                .ConValoracionesDisjuntas(true)
                .Build);

            Assert.Equal("No se puede generar una instancia con más agentes que átomos si las valoraciones son disjuntas", ex.Message);
        }

        [Fact]
        public void Build_AtomosYAgentesValidos_DevuelveMatrizCorrecta()
        {
            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(2)
                .Build();

            Assert.Equal(3, instancia.GetLength(0));
            Assert.Equal(2, instancia.GetLength(1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void Build_ValorMaximoConfigurado_RespetaLimiteSuperior(int valorMaximo)
        {
            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(valorMaximo)
                .Build();

            Assert.InRange(instancia[0, 0], 0, valorMaximo);
            Assert.InRange(instancia[0, 1], 0, valorMaximo);
            Assert.InRange(instancia[0, 2], 0, valorMaximo);
            Assert.InRange(instancia[1, 0], 0, valorMaximo);
            Assert.InRange(instancia[1, 1], 0, valorMaximo);
            Assert.InRange(instancia[1, 2], 0, valorMaximo);
            Assert.InRange(instancia[2, 0], 0, valorMaximo);
            Assert.InRange(instancia[2, 1], 0, valorMaximo);
            Assert.InRange(instancia[2, 2], 0, valorMaximo);
        }

        [Fact]
        public void Build_SinConfigurarValoracionesDisjuntas_CreaNoDisjuntasPorDefecto()
        {
            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(1)
                .ConValoracionesDisjuntas(false)
                .Build();

            Assert.NotEqual(0, instancia[0, 0]);
            Assert.NotEqual(0, instancia[0, 1]);
            Assert.NotEqual(0, instancia[0, 2]);
            Assert.NotEqual(0, instancia[1, 0]);
            Assert.NotEqual(0, instancia[1, 1]);
            Assert.NotEqual(0, instancia[1, 2]);
            Assert.NotEqual(0, instancia[2, 0]);
            Assert.NotEqual(0, instancia[2, 1]);
            Assert.NotEqual(0, instancia[2, 2]);
        }

        [Fact]
        public void Build_ValoracionesDisjuntas_CadaFilaTieneUnSoloValorPositivo()
        {
            const int agentes = 3;

            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(4)
                .ConCantidadDeAgentes(agentes)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
                .Build();

            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[0, j] > 0));
            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[1, j] > 0));
            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[2, j] > 0));
            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[3, j] > 0));
        }

        [Fact]
        public void Build_UnSoloAgenteValoracionesDisjuntas_AsignaTodaLaColumnaAlAgente()
        {
            const int agentes = 1;

            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(agentes)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
                .Build();

            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[0, j] > 0));
            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[1, j] > 0));
            Assert.Equal(1, Enumerable.Range(0, agentes).Count(j => instancia[2, j] > 0));
        }

        [Fact]
        public void Build_ValoracionesNoDisjuntas_NingunaCeldaValeCero()
        {
            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(1)
                .ConValoracionesDisjuntas(false)
                .Build();

            Assert.NotEqual(0, instancia[0, 0]);
            Assert.NotEqual(0, instancia[0, 1]);
            Assert.NotEqual(0, instancia[0, 2]);
            Assert.NotEqual(0, instancia[1, 0]);
            Assert.NotEqual(0, instancia[1, 1]);
            Assert.NotEqual(0, instancia[1, 2]);
            Assert.NotEqual(0, instancia[2, 0]);
            Assert.NotEqual(0, instancia[2, 1]);
            Assert.NotEqual(0, instancia[2, 2]);
        }
    }
}