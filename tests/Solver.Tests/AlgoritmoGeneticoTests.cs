namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_TamañoPoblacionInvalido_ArrojaArgumentException(int tamañoPoblacion)
        {
            var ex = Assert.Throws<ArgumentException>(() => new AlgoritmoGenetico(tamañoPoblacion));
            Assert.StartsWith($"El tamaño de la población debe ser positivo: {tamañoPoblacion}", ex.Message);
        }
    }
}
