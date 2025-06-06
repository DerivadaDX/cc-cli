using Common;
using NSubstitute;

namespace Generator.Tests
{
    public class InstanciaBuilderTests
    {
        private readonly InstanciaBuilder _instanciaBuilder;

        public InstanciaBuilderTests()
        {
            var generadorNumerosRandom = Substitute.For<GeneradorNumerosRandom>();
            _instanciaBuilder = new InstanciaBuilder(generadorNumerosRandom);
        }

        [Fact]
        public void Constructor_GeneradorNumerosRandomNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new InstanciaBuilder(null));
            Assert.Equal("generadorNumerosRandom", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeAtomos_CantidadInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConCantidadDeAtomos(cantidadAtomos));
            Assert.Contains("debe ser mayor que cero", ex.Message);
            Assert.Equal("cantidadAtomos", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConCantidadDeAgentes_CantidadInvalida_LanzaArgumentOutOfRangeException(int cantidadAgentes)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConCantidadDeAgentes(cantidadAgentes));
            Assert.Contains("debe ser mayor que cero", ex.Message);
            Assert.Equal("cantidadAgentes", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ConValorMaximo_CantidadInvalida_LanzaArgumentOutOfRangeException(int valorMaximo)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _instanciaBuilder.ConValorMaximo(valorMaximo));
            Assert.Contains("debe ser positivo", ex.Message);
            Assert.Equal("valorMaximo", ex.ParamName);
        }

        [Fact]
        public void Build_SinAtomosSeteados_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder.Build);
            Assert.Contains("especificar el número de átomos", ex.Message);
        }

        [Fact]
        public void Build_SinAgentesSeteados_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .Build);

            Assert.Contains("especificar el número de agentes", ex.Message);
        }

        [Fact]
        public void Build_SinValorMaximoSeteado_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .ConCantidadDeAgentes(2)
                .Build);

            Assert.Contains("especificar el valor máximo", ex.Message);
        }

        [Fact]
        public void Build_SinTipoDeInstanciaSeteado_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .ConCantidadDeAgentes(2)
                .ConValorMaximo(5)
                .Build);

            Assert.Contains("especificar el tipo", ex.Message);
        }

        [Fact]
        public void Build_ValoracionesDisjuntasConMasAgentesQueAtomos_LanzaInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(_instanciaBuilder
                .ConCantidadDeAtomos(2)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
                .Build);

            Assert.Contains("más agentes que átomos", ex.Message);
        }

        [Fact]
        public void Build_ConfiguracionCorrecta_DevuelveMatrizCorrecta()
        {
            decimal[,] instancia = _instanciaBuilder
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(2)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
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
                .ConValoracionesDisjuntas(false)
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
        public void Build_ValoracionesDisjuntas_CadaFilaTieneUnSoloValorPositivo()
        {
            var generadorNumerosRandom = Substitute.For<GeneradorNumerosRandom>();
            generadorNumerosRandom.Siguiente(Arg.Any<int>()).Returns(0);
            generadorNumerosRandom.Siguiente(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3, 4);

            decimal[,] instancia = new InstanciaBuilder(generadorNumerosRandom)
                .ConCantidadDeAtomos(4)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
                .Build();

            Assert.Equal(1, instancia[0, 0]);
            Assert.Equal(0, instancia[0, 1]);
            Assert.Equal(0, instancia[0, 2]);

            Assert.Equal(0, instancia[1, 0]);
            Assert.Equal(2, instancia[1, 1]);
            Assert.Equal(0, instancia[1, 2]);

            Assert.Equal(0, instancia[2, 0]);
            Assert.Equal(0, instancia[2, 1]);
            Assert.Equal(3, instancia[2, 2]);

            Assert.Equal(4, instancia[3, 0]);
            Assert.Equal(0, instancia[3, 1]);
            Assert.Equal(0, instancia[3, 2]);
        }

        [Fact]
        public void Build_UnSoloAgenteValoracionesDisjuntas_AsignaTodaLaColumnaAlAgente()
        {
            var generadorNumerosRandom = Substitute.For<GeneradorNumerosRandom>();
            generadorNumerosRandom.Siguiente(Arg.Any<int>()).Returns(0);
            generadorNumerosRandom.Siguiente(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);

            decimal[,] instancia = new InstanciaBuilder(generadorNumerosRandom)
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(1)
                .ConValorMaximo(5)
                .ConValoracionesDisjuntas(true)
                .Build();

            Assert.Equal(1, instancia[0, 0]);
            Assert.Equal(2, instancia[1, 0]);
            Assert.Equal(3, instancia[2, 0]);
        }

        [Fact]
        public void Build_ValoracionesNoDisjuntas_NingunaCeldaValeCero()
        {
            const int valorMaximo = 5;

            var generadorNumerosRandom = Substitute.For<GeneradorNumerosRandom>();
            generadorNumerosRandom.Siguiente(1, valorMaximo + 1).Returns(1);

            decimal[,] instancia = new InstanciaBuilder(generadorNumerosRandom)
                .ConCantidadDeAtomos(3)
                .ConCantidadDeAgentes(3)
                .ConValorMaximo(valorMaximo)
                .ConValoracionesDisjuntas(false)
                .Build();

            generadorNumerosRandom.Received(9).Siguiente(1, valorMaximo + 1);
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