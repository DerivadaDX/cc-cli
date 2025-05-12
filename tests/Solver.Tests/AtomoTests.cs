namespace Solver.Tests
{
    public class AtomoTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Posicion_DebeSerPositiva(int posicion)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Atomo(posicion, 1));
            Assert.StartsWith($"La posición debe ser positiva: {posicion}", ex.Message);
            Assert.Equal("posicion", ex.ParamName);
        }

        [Fact]
        public void Constructor_Valoracion_NoPuedeSerNegativa()
        {
            decimal valoracion = -0.1m;
            var ex = Assert.Throws<ArgumentException>(() => new Atomo(1, valoracion));
            Assert.StartsWith($"La valoración no puede ser negativa: {valoracion}", ex.Message);
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
