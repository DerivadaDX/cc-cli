namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Fact]
        public void Constructor_PoblacionNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(null));
            Assert.Equal("poblacion", ex.ParamName);
        }

        [Fact]
        public void Constructor_PoblacionVacia_ArrojaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new AlgoritmoGenetico([]));
            Assert.StartsWith("La población no puede estar vacía", ex.Message);
        }
    }
}
