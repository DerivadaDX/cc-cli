using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class IndividuoNuevoTests : IDisposable
    {
        public void Dispose()
        {
            AlgoritmoHungaroFactory.SetearInstancia(null);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(null, generadorRandom));

            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Constructor_GeneradorNumerosRandomNull_LanzaArgumentNullException()
        {
            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 5, cantidadAgentes: 3);
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(problema, null));

            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Contructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
        {
            int cantidadAtomos = 5;
            int cantidadAgentes = 3;
            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos, cantidadAgentes);
            IndividuoNuevo individuo = CrearIndividuo(problema);

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
            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos, cantidadAgentes);

            var generadorRandom1 = GeneradorNumerosRandomFactory.Crear(1);
            var generadorRandom2 = GeneradorNumerosRandomFactory.Crear(2);
            IndividuoNuevo individuo1 = CrearIndividuo(problema, generadorRandom1);
            IndividuoNuevo individuo2 = CrearIndividuo(problema, generadorRandom2);

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

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 4, cantidadAgentes: 4);
            IndividuoNuevo individuo = CrearIndividuo(problema);

            bool sonIguales = individuo.Asignaciones.SequenceEqual(asignacionOptima);
            Assert.True(sonIguales, "La asignación de porciones no coincide con la óptima esperada.");
        }

        [Fact]
        public void Constructor_ValoracionesDePorciones_SeCalculanYUsanLasCorrectas()
        {
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,] { { 1m, 2m }, { 3m, 4m } };
            calculadora.Calcular(Arg.Any<InstanciaProblema>(), Arg.Any<IReadOnlyList<int>>()).Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 3, cantidadAgentes: 2);
            CrearIndividuo(problema);

            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(valoraciones);
        }

        [Fact]
        public void Constructor_PosicionesDeCortes_CoincidenConLosGenesActivados()
        {
            IReadOnlyList<int> posicionesRecibidas = null;

            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            calculadora
                .Calcular(Arg.Any<InstanciaProblema>(), Arg.Do<IReadOnlyList<int>>(p => posicionesRecibidas = p));
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 5, cantidadAgentes: 3);
            IndividuoNuevo individuo = CrearIndividuo(problema, GeneradorNumerosRandomFactory.Crear(1));

            var posicionesEsperadas = individuo.Cromosoma
                .Select((gen, indice) => gen == 1 ? indice + 1 : 0)
                .Where(posicion => posicion > 0)
                .ToList();

            Assert.NotNull(posicionesRecibidas);
            Assert.Equal(posicionesEsperadas, posicionesRecibidas);
        }

        private IndividuoNuevo CrearIndividuo(InstanciaProblema problema, GeneradorNumerosRandom generadorRandom = null)
        {
            var generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(problema, generador);
            return individuo;
        }

        private InstanciaProblema CrearInstanciaProblema(int cantidadAtomos, int cantidadAgentes)
        {
            var matriz = new decimal[cantidadAtomos, cantidadAgentes];
            for (int atomo = 0; atomo < cantidadAtomos; atomo++)
            {
                for (int agente = 0; agente < cantidadAgentes; agente++)
                    matriz[atomo, agente] = atomo + agente + 1;
            }

            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            return instanciaProblema;
        }
    }
}
