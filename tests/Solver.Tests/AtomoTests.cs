namespace Solver.Tests
{
    public class AtomoTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_PosicionMenorAUno_LanzaArgumentOutOfRangeException(int posicion)
        {
            var excepcion = Record.Exception(() => new Atomo(posicion, 0.5m));

            var ex = Assert.IsType<ArgumentOutOfRangeException>(excepcion);
            Assert.Contains("debe ser mayor o igual a 1", ex.Message);
            Assert.Equal("posicion", ex.ParamName);
        }

        [Fact]
        public void Constructor_ValoracionNegativa_LanzaArgumentOutOfRangeException()
        {
            var excepcion = Record.Exception(() => new Atomo(1, -0.1m));

            var ex = Assert.IsType<ArgumentOutOfRangeException>(excepcion);
            Assert.Contains("no puede ser negativa", ex.Message);
            Assert.Equal("valoracion", ex.ParamName);
        }

        [Fact]
        public void Constructor_Posicion_SeAsigna()
        {
            int posicion = 1;
            var atomo = new Atomo(posicion, 0.5m);
            Assert.Equal(posicion, atomo.Posicion);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(.5)]
        [InlineData(10)]
        public void Constructor_Valoracion_SeAsigna(double valoracion)
        {
            var atomo = new Atomo(1, (decimal)valoracion);
            Assert.Equal((decimal)valoracion, atomo.Valoracion);
        }
    }
}
