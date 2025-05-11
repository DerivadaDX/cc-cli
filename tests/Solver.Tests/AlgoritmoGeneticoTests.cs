using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests
{
    public class AlgoritmoGeneticoTests
    {
        [Fact]
        public void Constructor_PoblacionNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(null, 10, _ => true));
            Assert.Equal("poblacion", ex.ParamName);
        }

        [Fact]
        public void Constructor_EsSolucionOptimaNull_ArrojaArgumentNullException()
        {
            var poblacion = Substitute.For<Poblacion>();
            var ex = Assert.Throws<ArgumentNullException>(() => new AlgoritmoGenetico(poblacion, 10, null));
            Assert.Equal("esSolucionOptima", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_MaxGeneracionesMenorOIgualACero_ArrojaArgumentOutOfRangeException(int maxGeneraciones)
        {
            var poblacion = Substitute.For<Poblacion>();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => new AlgoritmoGenetico(poblacion, maxGeneraciones, _ => true));

            Assert.Contains("El número máximo de generaciones debe ser mayor a cero.", ex.Message);
            Assert.Equal("maxGeneraciones", ex.ParamName);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_RetornaIndividuoOptimo()
        {
            var individuoOptimo = CrearIndividuoStub();
            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            Assert.Same(individuoOptimo, resultado);
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoGeneraMasPoblaciones()
        {
            var individuoOptimo = CrearIndividuoStub();
            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.DidNotReceive().GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EncuentraSolucionOptima_NoObtieneElMejorIndividuo()
        {
            var individuoOptimo = CrearIndividuoStub();
            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([individuoOptimo]);

            var algoritmo = new AlgoritmoGenetico(poblacion, 10, _ => true);
            Individuo resultado = algoritmo.Ejecutar();

            poblacion.DidNotReceive().ObtenerMejorIndividuo();
        }

        [Fact]
        public void Ejecutar_NoEncuentraSolucionOptima_RetornaMejorIndividuo()
        {
            Individuo mejorIndividuo = CrearIndividuoStub();
            var poblacion = Substitute.For<Poblacion>();
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
            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([CrearIndividuoStub()]);

            var nuevaPoblacion = Substitute.For<Poblacion>();
            nuevaPoblacion.Individuos.Returns([CrearIndividuoStub()]);
            poblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2, _ => false);
            algoritmo.Ejecutar();

            poblacion.Received(1).GenerarNuevaGeneracion();
            nuevaPoblacion.Received(1).GenerarNuevaGeneracion();
        }

        [Fact]
        public void Ejecutar_EvaluaTodosLosIndividuosEnCadaGeneracion()
        {
            Individuo individuo1 = CrearIndividuoStub();
            Individuo individuo2 = CrearIndividuoStub();

            var esSolucionOptima = Substitute.For<Func<Individuo, bool>>();
            esSolucionOptima(Arg.Any<Individuo>()).Returns(false);

            var poblacion = Substitute.For<Poblacion>();
            poblacion.Individuos.Returns([individuo1, individuo2]);

            var nuevaPoblacion = Substitute.For<Poblacion>();
            nuevaPoblacion.Individuos.Returns([individuo1, individuo2]);
            poblacion.GenerarNuevaGeneracion().Returns(nuevaPoblacion);

            var algoritmo = new AlgoritmoGenetico(poblacion, 2, esSolucionOptima);
            algoritmo.Ejecutar();

            esSolucionOptima.Received(2).Invoke(individuo1);
            esSolucionOptima.Received(2).Invoke(individuo2);
        }

        private Individuo CrearIndividuoStub()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var individuo = Substitute.For<Individuo>(new List<int> { 1, 1, 2 }, instanciaProblema);
            return individuo;
        }
    }
}
