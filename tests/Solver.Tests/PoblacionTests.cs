using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class PoblacionTests : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_TamañoInvalido_LanzaExcepcion(int tamaño)
        {
            var random = Substitute.For<GeneradorNumerosRandom>(1);
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Poblacion(tamaño, random));
            Assert.Contains("debe ser mayor a cero", ex.Message);
            Assert.Equal("tamaño", ex.ParamName);
        }

        [Fact]
        public void Constructor_GeneradorRandomNull_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Poblacion(3, null));
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void GenerarNuevaGeneracion_NuevaPoblacion_ConTamañoCorrecto()
        {
            int tamaño = 3;

            var poblacion = new Poblacion(tamaño, Substitute.For<GeneradorNumerosRandom>(1));
            poblacion.Individuos.AddRange([CrearIndividuoFake(), CrearIndividuoFake(), CrearIndividuoFake()]);

            Poblacion nuevaGeneracion = poblacion.GenerarNuevaGeneracion();
            Assert.Equal(tamaño, nuevaGeneracion.Individuos.Count);
        }

        [Fact]
        public void GenerarNuevaGeneracion_IncluyeElite()
        {
            Individuo mejorIndividuo = CrearIndividuoFake(fitness: 1);

            int tamaño = 3;
            var poblacion = new Poblacion(tamaño, Substitute.For<GeneradorNumerosRandom>(1));
            poblacion.Individuos.AddRange([mejorIndividuo, CrearIndividuoFake(fitness: 5), CrearIndividuoFake(fitness: 15)]);

            Poblacion nuevaGeneracion = poblacion.GenerarNuevaGeneracion();

            Assert.Contains(mejorIndividuo, nuevaGeneracion.Individuos);
        }

        [Fact]
        public void GenerarNuevaGeneracion_Padres_SonCruzados()
        {
            int tamaño = 2,
                indicePadre1 = 0,
                indicePadre2 = 1;

            var random = Substitute.For<GeneradorNumerosRandom>(1);
            random.Siguiente(tamaño).Returns(indicePadre1, indicePadre2);

            var poblacion = new Poblacion(tamaño, random);
            poblacion.Individuos.AddRange([CrearIndividuoFake(fitness: 5), CrearIndividuoFake(fitness: 15)]);
            poblacion.GenerarNuevaGeneracion();

            poblacion.Individuos[indicePadre1].Received(1).Cruzar(poblacion.Individuos[indicePadre2]);
        }

        [Fact]
        public void GenerarNuevaGeneracion_Hijos_Mutan()
        {
            var random = Substitute.For<GeneradorNumerosRandom>(1);
            random.Siguiente(Arg.Any<int>()).Returns(0, 1);

            Individuo padre = CrearIndividuoFake(fitness: 5);
            Individuo hijoMock = CrearIndividuoFake(fitness: 3);
            padre.Cruzar(Arg.Any<Individuo>()).Returns(hijoMock);

            var poblacion = new Poblacion(tamaño: 2, random);
            poblacion.Individuos.AddRange([padre, CrearIndividuoFake(fitness: 15)]);
            poblacion.GenerarNuevaGeneracion();

            hijoMock.Received(1).Mutar();
        }

        [Fact]
        public void GenerarNuevaGeneracion_PadresConIgualFitness_DesempataAlAzarEnTorneo()
        {
            // Torneo 1: elige índices 0 y 1, y en el empate devuelve 1 para seleccionar al segundo.
            // Torneo 2: elige dos veces el índice 2 para que el otro padre sea el tercer individuo.
            var random = Substitute.For<GeneradorNumerosRandom>(1);
            random.Siguiente(Arg.Any<int>()).Returns(0, 1, 1, 2, 2);

            Individuo padre1 = CrearIndividuoFake(fitness: 5);
            Individuo padre2 = CrearIndividuoFake(fitness: 5);
            Individuo padre3 = CrearIndividuoFake(fitness: 9);
            Individuo hijo = CrearIndividuoFake(fitness: 3);
            padre2.Cruzar(padre3).Returns(hijo);

            var poblacion = new Poblacion(tamaño: 3, random);
            poblacion.Individuos.AddRange([padre1, padre2, padre3]);

            poblacion.GenerarNuevaGeneracion();

            padre2.Received(1).Cruzar(padre3);
        }

        [Fact]
        public void ObtenerMejorIndividuo_Resultado_EsIndividuoConMenorFitness()
        {
            int mejorFitness = 5;

            var poblacion = new Poblacion(tamaño: 3, Substitute.For<GeneradorNumerosRandom>(1));
            poblacion.Individuos.AddRange(
                [CrearIndividuoFake(fitness: 15), CrearIndividuoFake(fitness: mejorFitness), CrearIndividuoFake(fitness: 10)]
            );

            Individuo mejorIndividuo = poblacion.ObtenerMejorIndividuo();
            Assert.Equal(mejorFitness, mejorIndividuo.Fitness());
        }

        private static Individuo CrearIndividuoFake(int fitness = 0)
        {
            var cromosoma = new List<int> { 1, 1, 2 };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                    { 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var individuo = Substitute.For<Individuo>(cromosoma, instanciaProblema, generadorRandom);
            var otroIndividuo = Substitute.For<Individuo>(cromosoma, instanciaProblema, generadorRandom);
            individuo.Cruzar(Arg.Any<Individuo>()).Returns(otroIndividuo);
            individuo.Fitness().Returns(fitness);
            return individuo;
        }
    }
}
