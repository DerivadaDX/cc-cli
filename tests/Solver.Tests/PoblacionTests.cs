using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class PoblacionTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_TamañoInvalido_LanzaExcepcion(int tamaño)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Poblacion(tamaño));
            Assert.Contains("debe ser mayor a cero", ex.Message);
            Assert.Equal("tamaño", ex.ParamName);
        }

        [Fact]
        public void GenerarNuevaGeneracion_NuevaPoblacion_ConTamañoCorrecto()
        {
            int tamaño = 3;

            Poblacion poblacion = new(tamaño);
            poblacion.Individuos.AddRange([
                CrearIndividuoStub(),
                CrearIndividuoStub(),
                CrearIndividuoStub()
            ]);

            Poblacion nuevaGeneracion = poblacion.GenerarNuevaGeneracion();
            Assert.Equal(tamaño, nuevaGeneracion.Individuos.Count);
        }

        [Fact]
        public void ObtenerMejorIndividuo_RetornaIndividuoConMenorFitness()
        {
            int mejorFitness = 5;

            Poblacion poblacion = new(tamaño: 3);
            poblacion.Individuos.AddRange([
                CrearIndividuoStub(fitness: 15),
                CrearIndividuoStub(fitness: mejorFitness),
                CrearIndividuoStub(fitness: 10)
            ]);

            Individuo mejorIndividuo = poblacion.ObtenerMejorIndividuo();
            Assert.Equal(mejorFitness, mejorIndividuo.Fitness);
        }

        private Individuo CrearIndividuoStub(int fitness = 0)
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var individuo = Substitute.For<Individuo>(new List<int> { 1, 1, 2 }, instanciaProblema);
            individuo.Cruzar(Arg.Any<Individuo>()).Returns(individuo);
            individuo.Fitness.Returns(fitness);
            return individuo;
        }
    }
}
