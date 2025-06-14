using NSubstitute;
using Solver.Individuos;
using System.Threading;

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

        [Fact]
        public void Constructor_LimiteGeneracionesNegativo_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new AlgoritmoGenetico(new Poblacion(1), -1));
            Assert.Contains("no puede ser negativo", ex.Message);
            Assert.Equal("limiteGeneraciones", ex.ParamName);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(individuoOptimo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            (Individuo mejorIndividuo, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuo);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoGeneraMasPoblaciones()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(individuoOptimo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            _ = algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoObtieneElMejorIndividuo()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(individuoOptimo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            _ = algoritmo.Ejecutar();

            poblacion.DidNotReceive().ObtenerMejorIndividuo();
        }

        [Fact]
        public void Ejecutar_NoEncuentraSolucionOptima_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);
            poblacion.ObtenerMejorIndividuo().Returns(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 1);
            (Individuo mejorIndividuoEncontrado, int _) = algoritmo.Ejecutar();

            poblacion.Received(1).ObtenerMejorIndividuo();
            Assert.Same(mejorIndividuo, mejorIndividuoEncontrado);
        }

        [Fact]
        public void Ejecutar_ConLimiteDeGeneraciones_GeneraCantidadEsperadaDeGeneraciones()
        {
            Poblacion poblacionInicial = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            Poblacion poblacionSiguiente = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);
            poblacionSiguiente.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 2);
            algoritmo.Ejecutar();

            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.Received(1).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_SinLimiteDeGeneraciones_EjecutaHastaEncontrarSolucionOptima()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacionInicial = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            Poblacion poblacionSiguiente = CrearPoblacionFakeConIndividuo(individuoOptimo);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 0);
            (Individuo mejorIndividuo, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuo);
            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_Generaciones_SeEvaluanTodosLosIndividuos()
        {
            Individuo individuo1 = CrearIndividuoNoOptimoFake();
            Individuo individuo2 = CrearIndividuoNoOptimoFake();

            var poblacionInicial = Substitute.For<Poblacion>(2);
            poblacionInicial.Individuos.Returns([individuo1, individuo2]);

            var poblacionSiguiente = Substitute.For<Poblacion>(2);
            poblacionSiguiente.Individuos.Returns([individuo1, individuo2]);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);
            poblacionSiguiente.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, 2);
            algoritmo.Ejecutar();

            individuo1.Received(2).Fitness();
            individuo2.Received(2).Fitness();
        }

        [Fact]
        public void Ejecutar_GeneracionesDevueltas_CoincideConGeneracionesGeneradas()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);
            poblacion.ObtenerMejorIndividuo().Returns(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 1);
            (Individuo _, int generaciones) = algoritmo.Ejecutar();

            Assert.Equal(1, generaciones);
        }

        [Fact]
        public void Ejecutar_EjecucionCancelada_RetornaMejorIndividuoActual()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.ObtenerMejorIndividuo().Returns(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            (Individuo individuo, int generaciones) = algoritmo.Ejecutar(cts.Token);

            Assert.Same(mejorIndividuo, individuo);
            Assert.Equal(0, generaciones);
        }

        private Individuo CrearIndividuoOptimoFake()
        {
            Individuo individuo = CrearIndividuoFake();
            individuo.Fitness().Returns(0);
            return individuo;
        }

        private Individuo CrearIndividuoNoOptimoFake()
        {
            Individuo individuo = CrearIndividuoFake();
            individuo.Fitness().Returns(1);
            return individuo;
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

        private Poblacion CrearPoblacionFakeConIndividuo(Individuo individuo)
        {
            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.Individuos.Returns([individuo]);
            return poblacion;
        }
    }
}
