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
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(problema, null));

            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Constructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            IndividuoNuevo individuo = CrearIndividuo(problema);

            int longitudCromosomaEsperada = 4;
            Assert.Equal(longitudCromosomaEsperada, individuo.Cromosoma.Count);

            int cantidadUnosEsperada = 2;
            int cantidadUnos = individuo.Cromosoma.Count(gen => gen == 1);
            Assert.Equal(cantidadUnosEsperada, cantidadUnos);

            int cantidadCerosEsperada = longitudCromosomaEsperada - cantidadUnosEsperada;
            int cantidadCeros = individuo.Cromosoma.Count(gen => gen == 0);
            Assert.Equal(cantidadCerosEsperada, cantidadCeros);
        }

        [Fact]
        public void Constructor_Cromosoma_UnosAsignadosAleatoriamente()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();

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
            var asignacionOptima = new List<int> { 0, 2, 1 };

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns(asignacionOptima);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            IndividuoNuevo individuo = CrearIndividuo(problema);

            bool sonIguales = individuo.Asignaciones.SequenceEqual(asignacionOptima);
            Assert.True(sonIguales, "La asignación de porciones no coincide con la óptima esperada.");
        }

        [Fact]
        public void Constructor_ValoracionesDePorciones_SeCalculanYUsanLasCorrectas()
        {
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,]
            {
                { 1m, 2m, 3m },
                { 4m, 5m, 6m },
                { 7m, 8m, 9m },
            };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            CrearIndividuo(problema);

            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(valoraciones);
        }

        [Fact]
        public void Constructor_PreferenciasPorcion_SeCalculanAPartirDeLasValoraciones()
        {
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,]
            {
                { 1m, 2m, 3m },
                { 4m, 5m, 6m },
                { 7m, 8m, 9m },
            };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            CrearIndividuo(problema);

            calculadora.Received(1).CalcularPreferenciasPorcion(valoraciones);
        }

        [Fact]
        public void Constructor_PosicionesDeCortes_CoincidenConLosGenesActivados()
        {
            List<int> posicionesRecibidas = null;

            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,]
            {
                { 1m, 2m, 3m },
                { 4m, 5m, 6m },
                { 7m, 8m, 9m },
            };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(
                    Arg.Any<InstanciaProblema>(), Arg.Do<List<int>>(p => posicionesRecibidas = p))
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0);
            CrearIndividuo(problema, generador);

            // Los cortes se generan en las posiciones 1 y 3 (del cromosoma [1, 0, 1, 0])
            var posicionesEsperadas = new List<int> { 1, 3 };
            Assert.Equal(posicionesEsperadas, posicionesRecibidas);
        }

        [Fact]
        public void Cruzar_OtroPadreNull_LanzaArgumentNullException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            IndividuoNuevo padre = CrearIndividuo(problema);

            var ex = Assert.Throws<ArgumentNullException>(() => padre.Cruzar(null));
            Assert.Equal("otro", ex.ParamName);
        }

        [Fact]
        public void Cruzar_CromosomasDeDistintaLongitud_LanzaArgumentException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            IndividuoNuevo padre = CrearIndividuo(problema);

            var problemaOtro = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m, 3m },
                { 2m, 3m, 4m },
                { 3m, 4m, 5m },
                { 4m, 5m, 6m },
            });
            IndividuoNuevo otroPadre = CrearIndividuo(problemaOtro);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("cromosomas no tienen la misma longitud", ex.Message);
        }

        [Fact]
        public void Cruzar_CantidadDeCortesDiferente_LanzaArgumentException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorPadre = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadre.Siguiente(Arg.Any<int>()).Returns(2, 0);
            IndividuoNuevo padre = CrearIndividuo(problema, generadorPadre);

            var problemaOtro = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m },
                { 2m, 3m },
                { 3m, 4m },
                { 4m, 5m },
                { 5m, 6m },
            });
            IndividuoNuevo otroPadre = CrearIndividuo(problemaOtro);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("no tienen la misma cantidad de cortes", ex.Message);
        }

        [Fact]
        public void Cruzar_CoincidenciasEnCerosYUnos_SeHeredan()
        {
            // Padre A cortes en 1, 3 y 5 → cromosoma [1, 0, 1, 0, 1, 0]
            // Padre B cortes en 1, 4 y 6 → cromosoma [1, 0, 0, 1, 0, 1]
            // Cortes en común: índice 0 (1) e índice 1 (0).
            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();

            var generadorPadreA = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreA.Siguiente(Arg.Any<int>()).Returns(4, 2, 0, 0, 0);
            IndividuoNuevo padreA = CrearIndividuo(problema, generadorPadreA);

            var generadorPadreB = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreB.Siguiente(Arg.Any<int>()).Returns(5, 3, 0);
            IndividuoNuevo padreB = CrearIndividuo(problema, generadorPadreB);

            IndividuoNuevo hijo = padreA.Cruzar(padreB);

            int indiceCorteEnComun = 0;
            Assert.Equal(padreA.Cromosoma[indiceCorteEnComun], padreB.Cromosoma[indiceCorteEnComun]);
            Assert.Equal(padreB.Cromosoma[indiceCorteEnComun], hijo.Cromosoma[indiceCorteEnComun]);

            int indiceNoCorteEnComun = 1;
            Assert.Equal(padreA.Cromosoma[indiceNoCorteEnComun], padreB.Cromosoma[indiceNoCorteEnComun]);
            Assert.Equal(padreB.Cromosoma[indiceNoCorteEnComun], hijo.Cromosoma[indiceNoCorteEnComun]);
        }

        [Fact]
        public void Cruzar_CortesFaltantes_CompletaAleatoriamenteHastaLaCantidadEsperada()
        {
            // Padre A cortes en 1, 3 y 5 → cromosoma [1, 0, 1, 0, 1, 0]
            // Padre B cortes en 1, 4 y 6 → cromosoma [1, 0, 0, 1, 0, 1]
            // Cortes diferentes: índices 2, 3, 4, 5. Se eligen 2 y 3.
            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();

            var generadorPadreA = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreA.Siguiente(Arg.Any<int>()).Returns(4, 2, 0, 0, 0);
            IndividuoNuevo padreA = CrearIndividuo(problema, generadorPadreA);

            var generadorPadreB = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreB.Siguiente(Arg.Any<int>()).Returns(5, 3, 0);
            IndividuoNuevo padreB = CrearIndividuo(problema, generadorPadreB);

            IndividuoNuevo hijo = padreA.Cruzar(padreB);

            var cromosomaEsperado = new List<int> { 1, 0, 1, 1, 0, 0 };
            Assert.Equal(cromosomaEsperado, hijo.Cromosoma);
        }

        [Fact]
        public void Cruzar_HijoIgualAPadre_HaceSwapDeUnCorte()
        {
            // Padre cortes en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Anti-clon: selecciona el 1 en índice 0 y el 0 en índice 3.
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();

            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 0, 0, 1);
            IndividuoNuevo padre = CrearIndividuo(problema, generador);

            IndividuoNuevo hijo = padre.Cruzar(padre);

            var cromosomaEsperado = new List<int> { 0, 0, 1, 1 };
            Assert.Equal(cromosomaEsperado, hijo.Cromosoma);
        }

        [Fact]
        public void Cruzar_Hijo_CalculaAsignacionesYPreferencias()
        {
            // Hijo cortes en 1, 3 y 4 → cromosoma [1, 0, 1, 1, 0, 0]
            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();

            // Padre A cortes en 1, 2 y 4 → cromosoma [1, 1, 0, 1, 0, 0]
            var generadorPadreA = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreA.Siguiente(Arg.Any<int>()).Returns(3, 1, 0, 2, 1);
            IndividuoNuevo padreA = CrearIndividuo(problema, generadorPadreA);

            // Padre B cortes en 1, 3 y 5 → cromosoma [1, 0, 1, 0, 1, 0]
            var generadorPadreB = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadreB.Siguiente(Arg.Any<int>()).Returns(4, 2, 0);
            IndividuoNuevo padreB = CrearIndividuo(problema, generadorPadreB);

            List<int> posicionesRecibidas = null;
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            var valoraciones = new decimal[,]
            {
                { 1m, 2m, 3m, 4m },
                { 2m, 3m, 4m, 5m },
                { 3m, 4m, 5m, 6m },
                { 4m, 5m, 6m, 7m },
            };
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(
                    Arg.Any<InstanciaProblema>(), Arg.Do<List<int>>(p => posicionesRecibidas = p))
                .Returns(valoraciones);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            IndividuoNuevo hijo = padreA.Cruzar(padreB);

            var posicionesEsperadas = new List<int> { 1, 3, 4 };
            Assert.Equal(posicionesEsperadas, posicionesRecibidas);
            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(valoraciones);
            calculadora.Received(1).CalcularPreferenciasPorcion(valoraciones);
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

            var cromosomaEsperado = new List<int> { 0, 1, 1, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
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

            var cromosomaEsperado = new List<int> { 0, 1, 1, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
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

            var cromosomaEsperado = new List<int> { 0, 1, 1, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
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

            var cromosomaEsperado = new List<int> { 1, 1, 0, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_MasDeUnaPorcionMasDeseada_SeleccionaAleatoriamenteUnaDeLasMasDeseadasYLaAchica()
        {
            // Cortes iniciales en 2, 4 y 6 → cromosoma [0, 1, 0, 1, 0, 1, 0]
            // Preferencias por porción: [0, 3, 3, 1]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m, 1m },
                { 1m, 1m, 1m, 1m },
                { 9m, 1m, 9m, 9m },
                { 9m, 1m, 9m, 9m },
                { 1m, 9m, 9m, 9m },
                { 1m, 9m, 9m, 9m },
                { 1m, 1m, 1m, 9m },
                { 1m, 1m, 1m, 9m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(
                5, 3, 1, // posiciones de cortes
                0 // desempate entre las más deseadas. [2, 1].
            );

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 0, 1, 0, 1, 1, 0, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaEnExtremoIzquierdo_AchicaDesdeAdentro()
        {
            // Cortes iniciales en 2 y 4 → cromosoma [0, 1, 0, 1]
            // Preferencias por porción: [3, 1, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 9m },
                { 1m, 1m, 9m },
                { 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(3, 1);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 0, 0, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
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

            var cromosomaEsperado = new List<int> { 1, 0, 0, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_AchicaLaSegundaMasDeseada()
        {
            // Cortes iniciales en 1 y 2 → cromosoma [1, 1, 0]
            // Preferencias por porción: [0, 3, 2]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 6m, 4m },
                { 2m, 3m, 2m },
                { 2m, 3m, 2m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(1, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 0, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_ConSegundasMasDeseadasEmpatadas_AchicaLaPorcionSeleccionadaAlAzar()
        {
            // Cortes iniciales en 1, 3 y 5 → cromosoma [1, 0, 1, 0, 1]
            // Preferencias por porción: [2, 1, 1, 0]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 9m, 1m, 1m },
                { 1m, 1m, 9m, 1m },
                { 1m, 1m, 9m, 1m },
                { 1m, 1m, 1m, 9m },
                { 1m, 1m, 1m, 9m },
                { 1m, 1m, 1m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(
                4, 2, 0, // posiciones de cortes
                0 // desempate entre segundas más deseadas. [2, 1].
            );

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 0, 1, 1, 0 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionesEmpatadasConSeleccionadaDeUnAtomo_AchicaLaSiguienteAleatoria()
        {
            // Cortes iniciales en 1 y 3 → cromosoma [1, 0, 1, 0]
            // Preferencias por porción: [3, 3, 3]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 4m, 4m, 4m },
                { 2m, 2m, 2m },
                { 2m, 2m, 2m },
                { 2m, 2m, 2m },
                { 2m, 2m, 2m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(
                2, 0, // posiciones de cortes
                1, 1 // desempate entre las más deseadas. [0, 2, 1].
            );

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 0, 0, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_RecorrePorcionesHastaEncontrarUnaAchicable()
        {
            // Cortes iniciales en 1, 2 y 3 → cromosoma [1, 1, 1, 0]
            // Preferencias por porción: [4, 4, 3, 2]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 7m, 4m, 2m },
                { 9m, 7m, 4m, 2m },
                { 5m, 7m, 4m, 2m },
                { 1m, 1m, 2m, 1m },
                { 1m, 1m, 2m, 1m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(2, 1, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 1, 0, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_TodasLasPorcionesDeUnAtomo_NoModificaElCromosoma()
        {
            // Cortes iniciales en 1 y 2 → cromosoma [1, 1]
            // Preferencias por porción: [1, 1, 1]
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m, 3m },
                { 1m, 2m, 3m },
                { 1m, 2m, 3m },
            });
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(1, 0);

            IndividuoNuevo individuo = CrearIndividuo(problema, generador);
            individuo.Mutar();

            var cromosomaEsperado = new List<int> { 1, 1 };
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
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

        private InstanciaProblema CrearInstanciaProblemaCincoAtomosTresAgentes()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m, 3m },
                { 2m, 3m, 4m },
                { 3m, 4m, 5m },
                { 4m, 5m, 6m },
                { 5m, 6m, 7m },
            });
            return instanciaProblema;
        }

        private InstanciaProblema CrearInstanciaProblemaSieteAtomosCuatroAgentes()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m, 3m, 4m },
                { 2m, 3m, 4m, 5m },
                { 3m, 4m, 5m, 6m },
                { 4m, 5m, 6m, 7m },
                { 5m, 6m, 7m, 8m },
                { 6m, 7m, 8m, 9m },
                { 7m, 8m, 9m, 10m },
            });
            return instanciaProblema;
        }
    }
}
