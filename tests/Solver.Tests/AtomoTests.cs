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
        }

        [Fact]
        public void Constructor_Valoracion_NoPuedeSerNegativa()
        {
            decimal valoracion = -0.1m;
            var ex = Assert.Throws<ArgumentException>(() => new Atomo(1, valoracion));
            Assert.StartsWith($"La valoración no puede ser negativa: {valoracion}", ex.Message);
        }

        [Fact]
        public void Constructor_Valoracion_NoPuedeSerMayorQueUno()
        {
            decimal valoracion = 1.1m;
            var ex = Assert.Throws<ArgumentException>(() => new Atomo(1, valoracion));
            Assert.StartsWith($"La valoración no puede ser mayor que 1: {valoracion}", ex.Message);
        }
    }
}
