using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoCortesBinariosTests : IDisposable
    {
        public void Dispose()
        {
            AlgoritmoHungaroFactory.SetearInstancia(null);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Constructor_CromosomaConCantidadGenesInvalida_LanzaArgumentException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoCortesBinarios([1, 0, 1], problema, generadorRandom));
            Assert.Contains("Cantidad de genes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaConCantidadUnosInvalida_LanzaArgumentException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoCortesBinarios([1, 0, 0, 0], problema, generadorRandom));
            Assert.Contains("Cantidad de cortes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaConGenesDistintosDeCeroOUno_LanzaArgumentException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoCortesBinarios([1, 0, 2, 0], problema, generadorRandom));
            Assert.Contains("solo puede contener genes 0 o 1", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CalculoDeValoracionesDePorciones_InvocaCalculadoraConProblemaYCortesCorrectos()
        {
            InstanciaProblema problemaRecibido = null;
            List<int> cortesRecibidos = null;

            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(
                    Arg.Do<InstanciaProblema>(p => problemaRecibido = p),
                    Arg.Do<List<int>>(c => cortesRecibidos = c))
                .Returns(new decimal[,]
                {
                    { 1m, 2m, 3m },
                    { 4m, 5m, 6m },
                    { 7m, 8m, 9m },
                });
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            _ = CrearIndividuo([1, 0, 1, 0], problema);

            Assert.Same(problema, problemaRecibido);
            Assert.Equal([1, 3], cortesRecibidos);
        }

        [Fact]
        public void Constructor_CalculoDeAsignacionDePorciones_InvocaAlgoritmoHungaroConValoracionesCorrectas()
        {
            decimal[,] valoracionesRecibidas = null;
            var valoracionesEsperadas = new decimal[,]
            {
                { 1m, 2m, 3m },
                { 4m, 5m, 6m },
                { 7m, 8m, 9m },
            };

            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(valoracionesEsperadas);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro
                .CalcularAsignacionOptimaDePorciones(Arg.Do<decimal[,]>(matriz => valoracionesRecibidas = matriz))
                .Returns([0, 1, 2]);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            _ = CrearIndividuo([1, 0, 1, 0], problema);

            Assert.Same(valoracionesEsperadas, valoracionesRecibidas);
        }

        [Fact]
        public void Constructor_AsignacionDePorciones_UsaResultadoDelAlgoritmoHungaro()
        {
            var asignacionOptima = new List<int> { 0, 2, 1 };

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns(asignacionOptima);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            IndividuoCortesBinarios individuo = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            Assert.Equal(asignacionOptima, individuo.Asignaciones);
            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>());
        }

        [Fact]
        public void ToString_Asignaciones_SeMuestranEnBaseUno()
        {
            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns([2, 0, 1]);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            IndividuoCortesBinarios individuo = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            string resultado = individuo.ToString();

            Assert.Contains("Asignaciones=[3, 1, 2]", resultado);
        }

        [Fact]
        public void ToString_FormatoGeneral_EsElEsperado()
        {
            var calculadora = Substitute.For<CalculadoraValoracionesPorciones>();
            calculadora
                .CalcularMatrizValoracionesPorcionAgente(Arg.Any<InstanciaProblema>(), Arg.Any<List<int>>())
                .Returns(new decimal[,]
                {
                    { 1m, 0m, 0m },
                    { 0m, 0m, 0m },
                    { 0m, 0m, 0m },
                });
            calculadora
                .CalcularPreferenciasPorcion(Arg.Any<decimal[,]>())
                .Returns([1, 1, 1]);
            CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadora);

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns([2, 0, 1]);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            IndividuoCortesBinarios individuo = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            string resultado = individuo.ToString();

            string esperado = "Cortes=[1, 0, 1, 0], Asignaciones=[3, 1, 2], Fitness=1.00";
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void Fitness_SinEnvidia_RetornaCero()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m, 0m },
                { 0m, 1m, 0m },
                { 0m, 0m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1], problema);

            Assert.Equal(0m, individuo.Fitness());
        }

        [Fact]
        public void Fitness_HayEnvidia_RetornaPositivo()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 5m, 5m, 5m },
                { 1m, 1m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1], problema);

            Assert.True(individuo.Fitness() > 0, $"Se esperaba un fitness positivo, pero se obtuvo {individuo.Fitness()}");
        }

        [Fact]
        public void Fitness_ValoracionesIguales_RetornaCero()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1], problema);

            Assert.Equal(0m, individuo.Fitness());
        }

        [Fact]
        public void Fitness_AsignacionPorcionAgenteEnBaseCero_CalculaValorEsperado()
        {
            // La asignación óptima de esta instancia es [1, 2, 0] (porción -> agente).
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 8m, 1m },
                { 8m, 2m, 9m },
                { 7m, 1m, 2m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1], problema);

            Assert.Equal(2m, individuo.Fitness());
        }

        [Fact]
        public void Cruzar_ConOtraFamiliaCromosoma_LanzaInvalidOperationException()
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            GeneradorNumerosRandom generador = GeneradorNumerosRandomFactory.Crear(1);

            Individuo padreNuevo = new IndividuoCortesBinarios([1, 0, 1, 0], problema, generador);
            Individuo otro = new IndividuoOtraFamiliaFake([1, 1, 2], problema, generador);

            var ex = Assert.Throws<InvalidOperationException>(() => padreNuevo.Cruzar(otro));
            Assert.Equal("No se puede cruzar individuos de familias de cromosoma diferentes.", ex.Message);
        }

        [Fact]
        public void Cruzar_OtroPadreNull_LanzaArgumentNullException()
        {
            IndividuoCortesBinarios padre = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            var ex = Assert.Throws<ArgumentNullException>(() => padre.Cruzar(null));
            Assert.Equal("otro", ex.ParamName);
        }

        [Fact]
        public void Cruzar_CromosomasDeDistintaLongitud_LanzaArgumentException()
        {
            IndividuoCortesBinarios padre = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            IndividuoCortesBinarios otroPadre = CrearIndividuoSieteAtomosCuatroAgentes([1, 0, 1, 0, 1, 0]);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("cromosomas no tienen la misma longitud", ex.Message);
        }

        [Fact]
        public void Cruzar_CantidadDeCortesDiferente_LanzaArgumentException()
        {
            IndividuoCortesBinarios padre = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            InstanciaProblema problemaOtro = CrearInstanciaProblemaCincoAtomosDosAgentes();
            IndividuoCortesBinarios otroPadre = CrearIndividuo([1, 0, 0, 0], problemaOtro);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("no tienen la misma cantidad de cortes", ex.Message);
        }

        [Fact]
        public void Cruzar_CoincidenciasEnCerosYUnos_SeHeredan()
        {
            List<int> cromosomaPadreA = [1, 0, 1, 0, 1, 0];
            List<int> cromosomaPadreB = [1, 0, 0, 1, 0, 1];

            IndividuoCortesBinarios padreA = CrearIndividuoSieteAtomosCuatroAgentes(cromosomaPadreA);
            IndividuoCortesBinarios padreB = CrearIndividuoSieteAtomosCuatroAgentes(cromosomaPadreB);

            IndividuoCortesBinarios hijo = padreA.Cruzar(padreB);

            int indiceCorteEnComun = 0;
            Assert.Equal(cromosomaPadreA[indiceCorteEnComun], hijo.Cromosoma[indiceCorteEnComun]);

            int indiceNoCorteEnComun = 1;
            Assert.Equal(cromosomaPadreA[indiceNoCorteEnComun], hijo.Cromosoma[indiceNoCorteEnComun]);
        }

        [Fact]
        public void Cruzar_CortesFaltantes_CompletaAleatoriamenteHastaLaCantidadEsperada()
        {
            // En el cruce elige 3 de [1, 2, 3, 4] (índice 5) y luego 2 de [1, 2, 3] (índice 4).
            var generadorPadres = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadres.Siguiente(Arg.Any<int>()).Returns(3, 2);

            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();
            var padreA = new IndividuoCortesBinarios([1, 0, 1, 0, 1, 0], problema, generadorPadres);
            var padreB = new IndividuoCortesBinarios([1, 0, 0, 1, 0, 1], problema, generadorPadres);

            IndividuoCortesBinarios hijo = padreA.Cruzar(padreB);

            List<int> cromosomaEsperado = [1, 0, 0, 0, 1, 1];
            Assert.Equal(cromosomaEsperado, hijo.Cromosoma);
        }

        [Fact]
        public void Cruzar_HijoIgualAPadre_HaceSwapDeUnCorte()
        {
            // El cruce entre iguales dispara anti-clon:
            // 0 -> toma el primer índice con 1 (índice 0), luego 1 -> toma el segundo índice con 0 (índice 3).
            // Se hace swap entre los índices 0 y 3 del cromosoma.
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(0, 1);

            IndividuoCortesBinarios padre = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0], generador);

            IndividuoCortesBinarios hijo = padre.Cruzar(padre);

            List<int> cromosomaEsperado = [0, 0, 1, 1];
            Assert.Equal(cromosomaEsperado, hijo.Cromosoma);
        }

        [Fact]
        public void Cruzar_Hijo_CalculaAsignacionesYPreferencias()
        {
            List<int> asignacionesEsperadas = [0, 1, 2, 3];

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns(asignacionesEsperadas);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            // En el cruce elige 3 de [1, 2, 3, 4] (índice 4) y luego 2 de [1, 2, 3] (índice 3).
            var generadorPadres = Substitute.For<GeneradorNumerosRandom>(1);
            generadorPadres.Siguiente(Arg.Any<int>()).Returns(3, 2);

            IndividuoCortesBinarios padreA = CrearIndividuoSieteAtomosCuatroAgentes([1, 1, 0, 1, 0, 0], generadorPadres);
            IndividuoCortesBinarios padreB = CrearIndividuoSieteAtomosCuatroAgentes([1, 0, 1, 0, 1, 0], generadorPadres);

            IndividuoCortesBinarios hijo = padreA.Cruzar(padreB);

            Assert.Equal(asignacionesEsperadas, hijo.Asignaciones);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaUnica_AchicaEsaPorcion()
        {
            // Preferencias por porción para cortes [1, 3]: [0, 3, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 1, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_VecinasEmpatadasYRandomDevuelveCero_AgrandaLaIzquierda()
        {
            // Preferencias por porción para cortes [1, 3]: [0, 3, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });

            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(2).Returns(0);

            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema, generador);

            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 1, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_VecinasEmpatadasYRandomDevuelveUno_AgrandaLaDerecha()
        {
            // Preferencias por porción para cortes [1, 3]: [0, 3, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });

            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(2).Returns(1);

            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema, generador);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 1, 0, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionIzquierdaMenosDeseada_AgrandaLaIzquierda()
        {
            // Preferencias por porción para cortes [1, 3]: [0, 2, 1].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 9m, 1m },
                { 9m, 9m, 1m },
                { 1m, 1m, 9m },
                { 1m, 1m, 9m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 1, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionDerechaMenosDeseada_AgrandaLaDerecha()
        {
            // Preferencias por porción para cortes [1, 3]: [1, 2, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 1m, 1m },
                { 1m, 5m, 5m },
                { 1m, 5m, 5m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 1, 0, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_MasDeUnaPorcionMasDeseadaDeIgualTamaño_SeleccionaAleatoriamenteUnaDeLasMasDeseadasYLaAchica()
        {
            // Preferencias por porción para cortes [2, 4, 6]: [0, 3, 3, 1].
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

            // En el empate [1, 2], ambas porciones tienen igual tamaño y se mezclan con Fisher-Yates.
            // Devolviendo 0, se intercambia el índice 1 por el 0: [1, 2] -> [2, 1].
            // Por eso se intenta achicar primero la porción 2.
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(0);

            var individuo = new IndividuoCortesBinarios([0, 1, 0, 1, 0, 1, 0], problema, generador);
            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 0, 1, 1, 0, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaEnExtremoIzquierdo_AchicaDesdeAdentro()
        {
            // Preferencias por porción para cortes [2, 4]: [3, 1, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
                { 1m, 1m, 9m },
                { 1m, 1m, 9m },
                { 1m, 1m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([0, 1, 0, 1], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 0, 0, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaEnExtremoDerecho_AchicaDesdeAdentro()
        {
            // Preferencias por porción para cortes [1, 3]: [0, 0, 3].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 9m, 9m, 9m },
                { 9m, 9m, 9m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 0, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 0, 0, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_AchicaLaSegundaMasDeseada()
        {
            // Preferencias por porción para cortes [1, 2]: [0, 3, 2].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 9m, 6m, 4m },
                { 2m, 3m, 2m },
                { 2m, 3m, 2m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 0, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomoConSegundasMasDeseadasEmpatadasDeIgualTamaño_AchicaLaPorcionSeleccionadaAlAzar()
        {
            // Preferencias por porción para cortes [1, 3, 5]: [2, 1, 1, 0].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 9m, 1m, 1m },
                { 1m, 1m, 9m, 1m },
                { 1m, 1m, 9m, 1m },
                { 1m, 1m, 1m, 9m },
                { 1m, 1m, 1m, 9m },
                { 1m, 1m, 1m, 1m },
            });

            // En el empate [1, 2], ambas porciones tienen igual tamaño.
            // Devolver 0 en Fisher-Yates produce [2, 1].
            var generador = Substitute.For<GeneradorNumerosRandom>(1);
            generador.Siguiente(Arg.Any<int>()).Returns(0);

            var individuo = new IndividuoCortesBinarios([1, 0, 1, 0, 1], problema, generador);
            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 0, 1, 1, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionesEmpatadasConDistintoTamaño_AchicaPrimeroLaMasGrande()
        {
            // Preferencias por porción para cortes [3, 5]: [3, 3, 3].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 2m, 2m, 2m },
                { 2m, 2m, 2m },
                { 4m, 4m, 4m },
                { 4m, 4m, 4m },
                { 4m, 4m, 4m },
                { 8m, 8m, 8m },
            });

            IndividuoCortesBinarios individuo = CrearIndividuo([0, 0, 1, 0, 1], problema);
            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 0, 0, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_PorcionMasDeseadaDeUnAtomo_RecorrePorcionesHastaEncontrarUnaAchicable()
        {
            // Preferencias por porción para cortes [1, 2, 3]: [4, 4, 3, 2].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 9m, 7m, 4m, 2m },
                { 9m, 7m, 4m, 2m },
                { 5m, 7m, 4m, 2m },
                { 1m, 1m, 2m, 1m },
                { 1m, 1m, 2m, 1m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1, 1, 0], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 1, 0, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_TodasLasPorcionesDeUnAtomo_NoModificaElCromosoma()
        {
            // Preferencias por porción para cortes [1, 2]: [1, 1, 1].
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m, 3m },
                { 1m, 2m, 3m },
                { 1m, 2m, 3m },
            });
            IndividuoCortesBinarios individuo = CrearIndividuo([1, 1], problema);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 1];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_AsignacionesDePorciones_SeRecalculan()
        {
            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro
                .CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>())
                .Returns([2, 1, 0], [0, 1, 2]);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            IndividuoCortesBinarios individuo = CrearIndividuoCincoAtomosTresAgentes([1, 0, 1, 0]);

            individuo.Mutar();

            List<int> asignacionesEsperadas = [0, 1, 2];
            Assert.Equal(asignacionesEsperadas, individuo.Asignaciones);
        }

        private IndividuoCortesBinarios CrearIndividuoCincoAtomosTresAgentes(
            List<int> cromosoma, GeneradorNumerosRandom generadorRandom = null)
        {
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var individuo = CrearIndividuo(cromosoma, problema, generadorRandom);
            return individuo;
        }

        private IndividuoCortesBinarios CrearIndividuoSieteAtomosCuatroAgentes(
            List<int> cromosoma, GeneradorNumerosRandom generadorRandom = null)
        {
            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();
            var individuo = CrearIndividuo(cromosoma, problema, generadorRandom);
            return individuo;
        }

        private IndividuoCortesBinarios CrearIndividuo(
            List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom = null)
        {
            GeneradorNumerosRandom generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoCortesBinarios(cromosoma, problema, generador);
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

        private InstanciaProblema CrearInstanciaProblemaCincoAtomosDosAgentes()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 2m },
                { 2m, 3m },
                { 3m, 4m },
                { 4m, 5m },
                { 5m, 6m },
            });
            return instanciaProblema;
        }

        private class IndividuoOtraFamiliaFake : Individuo
        {
            internal IndividuoOtraFamiliaFake(
                List<int> cromosoma,
                InstanciaProblema problema,
                GeneradorNumerosRandom generadorRandom)
                : base(cromosoma, problema, generadorRandom) { }

            protected override string FamiliaCromosoma => "legacy";

            internal override void Mutar()
            {
                throw new NotImplementedException();
            }

            internal override Individuo Cruzar(Individuo otro)
            {
                throw new NotImplementedException();
            }

            internal override decimal Fitness()
            {
                throw new NotImplementedException();
            }
        }
    }
}
