using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Fact]
        public void Constructor_PoblacionNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(null, 10));
            Assert.Equal("poblacion", ex.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        public void Constructor_MaxGeneracionesNegativo_LanzaArgumentOutOfRangeException(int maxGeneraciones)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AlgoritmoGenetico(new Poblacion(1), maxGeneraciones));

            Assert.Contains("no puede ser negativo", ex.Message);
            Assert.Equal("maxGeneraciones", ex.ParamName);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            Individuo individuoOptimo = CrearIndividuoFake();
            individuoOptimo.Fitness.Returns(0);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            Individuo resultado = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, resultado);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoGeneraMasPoblaciones()
        {
            Individuo individuoOptimo = CrearIndividuoFake();
            individuoOptimo.Fitness.Returns(0);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            _ = algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoObtieneElMejorIndividuo()
        {
            Individuo individuoOptimo = CrearIndividuoFake();
            individuoOptimo.Fitness.Returns(0);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            _ = algoritmo.Ejecutar();

            poblacion.DidNotReceive().ObtenerMejorIndividuo();
        }

        [Fact]
        public void Ejecutar_NoEncuentraSolucionOptima_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoFake();
            mejorIndividuo.Fitness.Returns(1);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([mejorIndividuo]);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);
            poblacion.ObtenerMejorIndividuo().Returns(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 1);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.Received(1).ObtenerMejorIndividuo();
            Assert.Same(mejorIndividuo, resultado);
        }

        [Fact]
        public void Ejecutar_GeneraNuevasGeneraciones_Correctamente()
        {
            var poblacion = Substitute.For<Poblacion>(1);
            Individuo individuoInicial = CrearIndividuoFake();
            individuoInicial.Fitness.Returns(1);
            poblacion.Individuos.Returns([individuoInicial]);

            var nuevaPoblacion = Substitute.For<Poblacion>(1);
            Individuo individuoSiguiente = CrearIndividuoFake();
            individuoSiguiente.Fitness.Returns(1);
            nuevaPoblacion.Individuos.Returns([individuoSiguiente]);
            poblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);
            nuevaPoblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2);
            algoritmo.Ejecutar();

            poblacion.Received(1).GenerarNuevaGeneracion();
            nuevaPoblacion.Received(1).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_MaxGeneracionesCero_EjecutaHastaEncontrarSolucion()
        {
            Individuo individuoOptimo = CrearIndividuoFake();
            individuoOptimo.Fitness.Returns(0);

            var poblacionInicial = Substitute.For<Poblacion>(1);
            Individuo individuoInicial = CrearIndividuoFake();
            individuoInicial.Fitness.Returns(1);
            poblacionInicial.Individuos.Returns([individuoInicial]);

            var poblacionSiguiente = Substitute.For<Poblacion>(1);
            poblacionSiguiente.Individuos.Returns([individuoOptimo]);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, 0);
            Individuo resultado = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, resultado);
            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.DidNotReceive().GenerarNuevaGeneracion();
        }

        private Individuo CrearIndividuoFake()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var individuo = Substitute.For<Individuo>(new List<int> { 1, 1, 2 }, instanciaProblema, new CalculadoraFitness());
            return individuo;
        }
    }
}
