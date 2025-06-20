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
        public void Ejecutar_EjecucionCancelada_RetornaMejorIndividuoActual()
        {
            (Poblacion poblacion, Individuo mejorIndividuo) = CrearPoblacionFakeConIndividuoNoOptimo();

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            (Individuo mejorIndividuoEncontrado, int generaciones) = algoritmo.Ejecutar(cts.Token);

            Assert.Same(mejorIndividuo, mejorIndividuoEncontrado);
            Assert.Equal(0, generaciones);
        }

        [Fact]
        public void Ejecutar_EjecucionCancelada_DetieneProcesamiento()
        {
            var generacionesNotificadas = new List<int>();
            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoNoOptimo();

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);

            using var cts = new CancellationTokenSource();
            cts.Cancel();
            algoritmo.Ejecutar(cts.Token);

            Assert.Empty(generacionesNotificadas);
            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_SinLimitesDeGeneraciones_EjecutaHastaEncontrarSolucionOptima()
        {
            (Poblacion poblacionInicial, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            (Poblacion poblacionSiguiente, Individuo individuoOptimo) = CrearPoblacionFakeConIndividuoOptimo();
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            (Individuo mejorIndividuoEncontrado, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuoEncontrado);
            poblacionInicial.Received(1).GenerarNuevaGeneracion();
            poblacionSiguiente.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            (Poblacion poblacion, Individuo individuoOptimo) = CrearPoblacionFakeConIndividuoOptimo();

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            (Individuo mejorIndividuoEncontrado, int _) = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, mejorIndividuoEncontrado);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_DetieneProcesamiento()
        {
            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoOptimo();

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 0);
            algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_HayEstancamiento_NotificaDetencion()
        {
            bool notificado = false;

            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 5);
            algoritmo.EstancamientoDetectado += () => notificado = true;
            algoritmo.Ejecutar();

            Assert.True(notificado);
        }

        [Fact]
        public void Ejecutar_ConLimiteDeGeneraciones_ProcesaCantidadEsperadaDeGeneraciones()
        {
            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 10, limiteGeneracionesSinMejora: 0);
            algoritmo.Ejecutar();

            // En este test la población se devuelve a sí misma en cada generación, por lo que recibe todos los llamados
            poblacion.Received(10).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_GeneracionLimiteAlcanzada_RetornaMejorIndividuoYCantidadCorrectaDeGeneraciones()
        {
            (Poblacion poblacion, Individuo mejorIndividuo) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 10, limiteGeneracionesSinMejora: 0);
            (Individuo mejorIndividuoEncontrado, int generaciones) = algoritmo.Ejecutar();

            Assert.Same(mejorIndividuo, mejorIndividuoEncontrado);
            Assert.Equal(10, generaciones);
        }

        [Fact]
        public void Ejecutar_HayEstancamiento_RetornaMejorIndividuo()
        {
            (Poblacion poblacion, Individuo mejorIndividuo) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 0, limiteGeneracionesSinMejora: 10);
            (Individuo individuo, int generaciones) = algoritmo.Ejecutar();

            Assert.Same(mejorIndividuo, individuo);
            Assert.Equal(10, generaciones);
            poblacion.Received(10).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_GeneracionesDevueltas_CoincideConGeneracionesProcesadas()
        {
            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 10, limiteGeneracionesSinMejora: 0);
            (_, int generaciones) = algoritmo.Ejecutar();

            Assert.Equal(10, generaciones);
        }

        [Fact]
        public void Ejecutar_GeneracionProcesada_NotificaNumeroDeGeneracion()
        {
            var generacionesNotificadas = new List<int>();

            (Poblacion poblacion, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            poblacion.GenerarNuevaGeneracion().Returns(poblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, limiteGeneraciones: 5, limiteGeneracionesSinMejora: 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);
            algoritmo.Ejecutar();

            Assert.Equal([1, 2, 3, 4, 5], generacionesNotificadas);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NotificaSoloGeneracionesHastaSolucion()
        {
            var generacionesNotificadas = new List<int>();

            (Poblacion poblacionInicial, _) = CrearPoblacionFakeConIndividuoNoOptimo();
            (Poblacion poblacionSiguiente, _) = CrearPoblacionFakeConIndividuoOptimo();
            poblacionInicial.GenerarNuevaGeneracion().Returns(poblacionSiguiente);

            var algoritmo = new AlgoritmoGenetico(poblacionInicial, limiteGeneraciones: 10, limiteGeneracionesSinMejora: 0);
            algoritmo.GeneracionProcesada += (generacion, _) => generacionesNotificadas.Add(generacion);
            algoritmo.Ejecutar();

            Assert.Equal([1], generacionesNotificadas);
        }

        private (Poblacion poblacion, Individuo individuo) CrearPoblacionFakeConIndividuoNoOptimo()
        {
            Individuo individuo = CrearIndividuoFake();
            individuo.Fitness().Returns(1);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.ObtenerMejorIndividuo().Returns(individuo);

            return (poblacion, individuo);
        }

        private (Poblacion poblacion, Individuo individuo) CrearPoblacionFakeConIndividuoOptimo()
        {
            Individuo individuo = CrearIndividuoFake();
            individuo.Fitness().Returns(0);

            var poblacion = Substitute.For<Poblacion>(1);
            poblacion.ObtenerMejorIndividuo().Returns(individuo);

            return (poblacion, individuo);
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
