using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class IndividuoNuevoTests : IDisposable
    {
        public void Dispose()
        {
            AlgoritmoHungaroFactory.SetearInstancia(null);
            GC.SuppressFinalize(this);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_CantidadAtomosInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CrearIndividuo(cantidadAtomos, cantidadAgentes: 1));
            Assert.Contains("debe ser mayor o igual a 1", ex.Message);
            Assert.Equal("cantidadAtomos", ex.ParamName);
        }

        [Fact]
        public void Constructor_CantidadAgentesMayorQueCantidadAtomos_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CrearIndividuo(cantidadAtomos: 5, cantidadAgentes: 6));
            Assert.Contains("no puede ser mayor que la cantidad de átomos", ex.Message);
            Assert.Equal("cantidadAgentes", ex.ParamName);
        }

        [Fact]
        public void Constructor_GeneradorNumerosRandomNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(cantidadAtomos: 5, cantidadAgentes: 3, null));
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Contructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
        {
            int cantidadAtomos = 5;
            int cantidadAgentes = 3;
            IndividuoNuevo individuo = CrearIndividuo(cantidadAtomos, cantidadAgentes);

            int longitudCromosomaEsperada = cantidadAtomos - 1;
            Assert.Equal(longitudCromosomaEsperada, individuo.Cromosoma.Count);

            int cantidadUnosEsperada = cantidadAgentes - 1;
            int cantidadUnos = individuo.Cromosoma.Count(gen => gen == 1);
            Assert.Equal(cantidadUnosEsperada, cantidadUnos);

            int cantidadCerosEsperada = longitudCromosomaEsperada - cantidadUnosEsperada;
            int cantidadCeros = individuo.Cromosoma.Count(gen => gen == 0);
            Assert.Equal(cantidadCerosEsperada, cantidadCeros);
        }

        [Fact]
        public void Constructor_Cromosoma_UnosAsignadosAleatoriamente()
        {
            int cantidadAtomos = 10;
            int cantidadAgentes = 4;
            var generadorRandom1 = GeneradorNumerosRandomFactory.Crear(1);
            var generadorRandom2 = GeneradorNumerosRandomFactory.Crear(2);
            IndividuoNuevo individuo1 = CrearIndividuo(cantidadAtomos, cantidadAgentes, generadorRandom1);
            IndividuoNuevo individuo2 = CrearIndividuo(cantidadAtomos, cantidadAgentes, generadorRandom2);

            bool sonIguales = individuo1.Cromosoma.SequenceEqual(individuo2.Cromosoma);
            Assert.False(sonIguales, "Se esperaban cromosomas diferentes entre dos instancias.");
        }

        [Fact]
        public void Constructor_AsignacionDePorciones_InicializadaConLaOptima()
        {
            int[] asignacionOptima = [0, 2, 1, 3];

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns(asignacionOptima);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            IndividuoNuevo individuo = CrearIndividuo(cantidadAtomos: 10, cantidadAgentes: 4, generadorRandom);

            bool sonIguales = individuo.Asignaciones.SequenceEqual(asignacionOptima);
            Assert.True(sonIguales, "La asignación de porciones no coincide con la óptima esperada.");
        }

        private IndividuoNuevo CrearIndividuo(int cantidadAtomos, int cantidadAgentes, GeneradorNumerosRandom generadorRandom = null)
        {
            var generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cantidadAtomos, cantidadAgentes, generador);
            return individuo;
        }

        /*
         * Tests faltantes:
         * - Antes de calcular la asignación óptima de porciones, la matriz de valoraciones es correcta
         * - Inicializar el cromosoma calcula la asignación óptima de porciones
         * - Mutar mantiene la cantidad correcta de unos y ceros
         * - Mutar cambia la posición de algunos unos y ceros si cumple la probabilidad
         * - Mutar produce diferentes resultados en distintas ejecuciones
         * - Mutar actualiza la asignación óptima de porciones
         * - Cruzar produce un hijo con la cantidad correcta de unos y ceros
         * - Cruzar produce un hijo con genes tomados de ambos padres
         */
    }
}
