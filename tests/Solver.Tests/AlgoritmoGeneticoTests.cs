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
        public void Ejecutar_PoblacionDeUnSoloIndividuo_DevuelveElMismoIndividuo()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var individuo = Substitute.For<Individuo>(new List<int> { 1, 1, 2 }, instanciaProblema);
            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([individuo]);

            var algoritmoGenetico = new AlgoritmoGenetico(poblacion);
            Individuo resultado = algoritmoGenetico.Ejecutar();

            Assert.Same(individuo, resultado);
        }
    }
}
