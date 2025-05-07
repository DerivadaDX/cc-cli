using NSubstitute;
using Solver.Individuos;

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

        [Fact]
        public void Ejecutar_PoblacionDeUnSoloIndividuo_DevuelveElMismoIndividuo()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var individuo = Substitute.For<Individuo>(new List<int> { 1, 1, 2 }, instanciaProblema);

            var algoritmoGenetico = new AlgoritmoGenetico([individuo]);
            Individuo resultado = algoritmoGenetico.Ejecutar();

            Assert.Same(individuo, resultado);
        }
    }
}
