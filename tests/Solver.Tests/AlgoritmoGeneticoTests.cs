using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Fact]
        public void Constructor_PoblacionNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(null, 10, _ => true));
            Assert.Equal("poblacion", ex.ParamName);
        }

        [Fact]
        public void Constructor_EsSolucionOptimaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(new Poblacion(1), 10, null));
            Assert.Equal("esSolucionOptima", ex.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        public void Constructor_MaxGeneracionesNegativo_LanzaArgumentOutOfRangeException(int maxGeneraciones)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AlgoritmoGenetico(new Poblacion(1), maxGeneraciones, _ => true));

            Assert.Contains("no puede ser negativo", ex.Message);
            Assert.Equal("maxGeneraciones", ex.ParamName);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            var individuoOptimo = CrearIndividuoFake();
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, resultado);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoGeneraMasPoblaciones()
        {
            var individuoOptimo = CrearIndividuoFake();
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoObtieneElMejorIndividuo()
        {
            var individuoOptimo = CrearIndividuoFake();
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.DidNotReceive().ObtenerMejorIndividuo();
        }

        [Fact]
        public void Ejecutar_NoEncuentraSolucionOptima_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoFake();
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([mejorIndividuo]);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);
            poblacion.ObtenerMejorIndividuo().Returns(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 1, _ => false);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.Received(1).ObtenerMejorIndividuo();
            Assert.Same(mejorIndividuo, resultado);
        }

        [Fact]
        public void Ejecutar_GeneraNuevasGeneraciones_Correctamente()
        {
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([CrearIndividuoFake()]);

            var nuevaPoblacion = Substitute.For<Poblacion>(1);
            nuevaPoblacion.Individuos.Returns([CrearIndividuoFake()]);
            poblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);
            nuevaPoblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2, _ => false);
            algoritmo.Ejecutar();

            poblacion.Received(1).GenerarNuevaGeneracion();
            nuevaPoblacion.Received(1).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_MaxGeneracionesCero_EjecutaHastaEncontrarSolucion()
        {
            Individuo individuoOptimo = CrearIndividuoFake();

            var poblacionInicial = Substitute.For<Poblacion>(1);
            poblacionInicial.Individuos.Returns([CrearIndividuoFake()]);

            var poblacionSiguiente = Substitute.For<Poblacion>(1);
            poblacionSiguiente.Individuos.Returns([individuoOptimo]);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, 0, i => i == individuoOptimo);
            Individuo resultado = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, resultado);
            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EvaluaTodosLosIndividuosEnCadaGeneracion()
        {
            Individuo individuo1 = CrearIndividuoFake();
            Individuo individuo2 = CrearIndividuoFake();

            var esSolucionOptima = Substitute.For<Func<Individuo, bool>>();
            esSolucionOptima(Arg.Any<Individuo>()).Returns(false);

            var poblacion = Substitute.For<Poblacion>(2);
            poblacion.Individuos.Returns([individuo1, individuo2]);

            var nuevaPoblacion = Substitute.For<Poblacion>(2);
            nuevaPoblacion.Individuos.Returns([individuo1, individuo2]);
            poblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);
            nuevaPoblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2, esSolucionOptima);
            algoritmo.Ejecutar();

            esSolucionOptima.Received(2).Invoke(individuo1);
            esSolucionOptima.Received(2).Invoke(individuo2);
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
