namespace Solver.Tests
{
    public class AtomoTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Posicion_DebeSerPositiva(int posicion)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Atomo(posicion));
            Assert.StartsWith("La posición debe ser positiva", ex.Message);
        }
    }
}
