using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Fact]
        public void Constructor_PoblacionNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(null, 10, 0));
            Assert.Equal("poblacion", ex.ParamName);
        }

        [Fact]
        public void Constructor_LimiteGeneracionesNegativo_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new AlgoritmoGenetico(new Poblacion(1), -1, 0));
            Assert.Contains("no puede ser negativo", ex.Message);
            Assert.Equal("limiteGeneraciones", ex.ParamName);
        }

        [Fact]
        public void Constructor_LimiteGeneracionesSinMejoraNegativo_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new AlgoritmoGenetico(new Poblacion(1), 1, -1));
            Assert.Contains("no puede ser negativo", ex.Message);
            Assert.Equal("limiteGeneracionesSinMejora", ex.ParamName);
        }

        [Fact]
        public void Ejecutar_SinLimitesDeGeneraciones_EjecutaHastaEncontrarSolucionOptima()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacionInicial = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            Poblacion poblacionSiguiente = CrearPoblacionFakeConIndividuo(individuoOptimo);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            (Individuo mejorIndividuo, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuo);
            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(individuoOptimo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, 0);
            (Individuo mejorIndividuo, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuo);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoGeneraMasPoblaciones()
        {
            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(individuoOptimo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, 0);
            _ = algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_ConLimiteDeGeneraciones_GeneraCantidadEsperadaDeGeneraciones()
        {
            Poblacion poblacionInicial = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            Poblacion poblacionSiguiente = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);
            poblacionSiguiente.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 2, 0);
            algoritmo.Ejecutar();

            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.Received(1).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_GeneracionLimiteAlcanzada_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 1, 0);
            (Individuo mejorIndividuoEncontrado, int _) = algoritmo.Ejecutar();

            Assert.Same(mejorIndividuo, mejorIndividuoEncontrado);
        }

        [Fact]
        public void Ejecutar_HayEstancamiento_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 100, limiteGeneracionesSinMejora: 2);
            (Individuo individuo, int generaciones) = algoritmo.Ejecutar();

            Assert.Same(mejorIndividuo, individuo);
            Assert.Equal(2, generaciones);
            poblacion.Received(2).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_GeneracionesDevueltas_CoincideConGeneracionesGeneradas()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 1, 0);
            (Individuo _, int generaciones) = algoritmo.Ejecutar();

            Assert.Equal(1, generaciones);
        }

        [Fact]
        public void Ejecutar_EjecucionCancelada_RetornaMejorIndividuoActual()
        {
            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, 0);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            (Individuo individuo, int generaciones) = algoritmo.Ejecutar(cts.Token);

            Assert.Same(mejorIndividuo, individuo);
            Assert.Equal(0, generaciones);
        }

        [Fact]
        public void Ejecutar_GeneracionProcesada_NotificaConGeneracionCorrecta()
        {
            var generacionesNotificadas = new List<int>();

            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2, 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);

            algoritmo.Ejecutar();

            Assert.Equal([1, 2], generacionesNotificadas);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NotificaSoloGeneracionesHastaSolucion()
        {
            var generacionesNotificadas = new List<int>();

            Individuo individuoOptimo = CrearIndividuoOptimoFake();
            Poblacion poblacionInicial = CrearPoblacionFakeConIndividuo(CrearIndividuoNoOptimoFake());
            Poblacion poblacionSiguiente = CrearPoblacionFakeConIndividuo(individuoOptimo);
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, 5, 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);

            algoritmo.Ejecutar();

            Assert.Equal([1], generacionesNotificadas);
        }

        [Fact]
        public void Ejecutar_EjecucionCancelada_NoNotificaGeneraciones()
        {
            var generacionesNotificadas = new List<int>();

            Individuo mejorIndividuo = CrearIndividuoNoOptimoFake();
            Poblacion poblacion = CrearPoblacionFakeConIndividuo(mejorIndividuo);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);

            using var cts = new CancellationTokenSource();
            cts.Cancel();
            algoritmo.Ejecutar(cts.Token);

            Assert.Empty(generacionesNotificadas);
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
            poblacion.ObtenerMejorIndividuo().Returns(individuo);
            return poblacion;
        }
    }
}
