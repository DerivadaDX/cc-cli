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
            var asignacionOptima = new List<int> { 0, 2, 1, 3 };

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
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 3, cantidadAgentes: 2);
            CrearIndividuo(problema);

            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(valoraciones);
        }

        [Fact]
        public void Constructor_PreferenciasPorcion_SeCalculanAPartirDeLasValoraciones()
        {
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,] { { 1m, 2m }, { 3m, 4m } };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 3, cantidadAgentes: 2);
            CrearIndividuo(problema);

            calculadora.Received(1).CalcularPreferenciasPorcion(valoraciones);
        }

        [Fact]
        public void Constructor_PosicionesDeCortes_CoincidenConLosGenesActivados()
        {
            List<int> posicionesRecibidas = null;

            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,] { { 1m } };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(
                    Arg.Any<InstanciaProblema>(), Arg.Do<List<int>>(p => posicionesRecibidas = p))
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblema(cantidadAtomos: 5, cantidadAgentes: 3);
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0); // Selecciona índices 2 y 0 para los unos en el cromosoma
            CrearIndividuo(problema, generador);

            // Los unos en el cromosoma estarán en las posiciones 1 y 3 (índices 0-based)
            Assert.Equal([1, 3], posicionesRecibidas);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaUnica_AchicaEsaPorcion()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [0, 3, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([0, 1, 1, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionesVecinasEmpatadas_AgrandaLaIzquierda()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [0, 3, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([0, 1, 1, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionIzquierdaMenosDeseada_AgrandaLaIzquierda()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [0, 2, 1]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 1m },
                { 9m, 9m, 1m },
                { 1m, 1m, 9m },
                { 1m, 1m, 9m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([0, 1, 1, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionDerechaMenosDeseada_AgrandaLaDerecha()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [1, 2, 0]
            var valoraciones = new decimal[,]
            {
                { 9m, 1m, 1m },
                { 1m, 5m, 5m },
                { 1m, 5m, 5m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([1, 1, 0, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_MasDeUnaPorcionMasDeseada_AchicaLaPrimera()
        {
            // Cortes iniciales en 2, 4 y 6 → cromosoma [0, 1, 0, 1, 0, 1, 0]
            // Preferencias por porción: [0, 3, 3, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m, 1m },
                { 1m, 1m, 1m, 1m },
                { 9m, 1m, 9m, 9m },
                { 9m, 1m, 9m, 9m },
                { 1m, 9m, 9m, 9m },
                { 1m, 9m, 9m, 9m },
                { 1m, 1m, 1m, 1m },
                { 1m, 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(5, 3, 1);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([0, 0, 1, 1, 0, 1, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaEnExtremoIzquierdo_AchicaDesdeAdentro()
        {
            // Cortes iniciales en 2 y 4 → cromosoma [0, 1, 0, 1]
            // Preferencias por porción: [3, 0, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(1, 2);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([1, 0, 0, 1], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaEnExtremoDerecho_AchicaDesdeAdentro()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [0, 0, 3]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([1, 0, 0, 1], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_NoModificaElCromosoma()
        {
            // Cortes iniciales en 1 y 2 → cromosoma [1, 1, 0]
            // Preferencias por porción: [0, 3, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(1, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            Assert.Equal([1, 1, 0], individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_AsignacionesDePorciones_SeRecalculan()
        {
            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);
            
            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            algoritmoHungaro.Received(2).CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>());
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
