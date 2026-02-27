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
        public void Constructor_CromosomaNull_LanzaArgumentNullException()
        {
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(null, problema, generadorRandom));
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            List<int> cromosoma = [1, 0, 1, 0];
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(cromosoma, null, generadorRandom));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Constructor_GeneradorNumerosRandomNull_LanzaArgumentNullException()
        {
            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(cromosoma, problema, null));
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaConCantidadGenesInvalida_LanzaArgumentException()
        {
            List<int> cromosoma = [1, 0, 1];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoNuevo(cromosoma, problema, generadorRandom));
            Assert.Contains("Cantidad de genes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaConCantidadUnosInvalida_LanzaArgumentException()
        {
            List<int> cromosoma = [1, 0, 0, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoNuevo(cromosoma, problema, generadorRandom));
            Assert.Contains("Cantidad de cortes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaConGenesDistintosDeCeroOUno_LanzaArgumentException()
        {
            List<int> cromosoma = [1, 0, 2, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoNuevo(cromosoma, problema, generadorRandom));
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

            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            _ = new IndividuoNuevo(cromosoma, problema, generadorRandom);

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

            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            _ = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.Same(valoracionesEsperadas, valoracionesRecibidas);
        }

        [Fact]
        public void Constructor_AsignacionDePorciones_UsaResultadoDelAlgoritmoHungaro()
        {
            var asignacionOptima = new List<int> { 0, 2, 1 };

            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns(asignacionOptima);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.Equal(asignacionOptima, individuo.Asignaciones);
            algoritmoHungaro.Received(1).CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>());
        }

        [Fact]
        public void ToString_Asignaciones_SeMuestranEnBaseUno()
        {
            var algoritmoHungaro = Substitute.For<AlgoritmoHungaro>();
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns([2, 0, 1]);
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaro);

            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

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

            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            string resultado = individuo.ToString();

            string esperado = "Cortes=[1, 0, 1, 0], Asignaciones=[3, 1, 2], Fitness=1";
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
            List<int> cromosoma = [1, 1];
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.Equal(0m, individuo.Fitness);
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
            List<int> cromosoma = [1, 1];
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.True(individuo.Fitness > 0, $"Se esperaba un fitness positivo, pero se obtuvo {individuo.Fitness}");
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
            List<int> cromosoma = [1, 1];
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.Equal(0m, individuo.Fitness);
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
            List<int> cromosoma = [1, 1];
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            Assert.Equal(2m, individuo.Fitness);
        }

        [Fact]
        public void Cruzar_OtroPadreNull_LanzaArgumentNullException()
        {
            List<int> cromosoma = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var padre = new IndividuoNuevo(cromosoma, problema, generadorRandom);

            var ex = Assert.Throws<ArgumentNullException>(() => padre.Cruzar(null));
            Assert.Equal("otro", ex.ParamName);
        }

        [Fact]
        public void Cruzar_CromosomasDeDistintaLongitud_LanzaArgumentException()
        {
            List<int> cromosomaPadre = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandomPadre = GeneradorNumerosRandomFactory.Crear(1);
            var padre = new IndividuoNuevo(cromosomaPadre, problema, generadorRandomPadre);

            List<int> cromosomaOtroPadre = [1, 0, 1, 0, 1, 0];
            InstanciaProblema problemaOtro = CrearInstanciaProblemaSieteAtomosCuatroAgentes();
            var generadorRandomOtroPadre = GeneradorNumerosRandomFactory.Crear(1);
            var otroPadre = new IndividuoNuevo(cromosomaOtroPadre, problemaOtro, generadorRandomOtroPadre);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("cromosomas no tienen la misma longitud", ex.Message);
        }

        [Fact]
        public void Cruzar_CantidadDeCortesDiferente_LanzaArgumentException()
        {
            List<int> cromosomaPadre = [1, 0, 1, 0];
            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var generadorRandomPadre = GeneradorNumerosRandomFactory.Crear(1);
            var padre = new IndividuoNuevo(cromosomaPadre, problema, generadorRandomPadre);

            List<int> cromosomaOtroPadre = [1, 0, 0, 0];
            InstanciaProblema problemaOtro = CrearInstanciaProblemaCincoAtomosDosAgentes();
            var generadorRandomOtroPadre = GeneradorNumerosRandomFactory.Crear(1);
            var otroPadre = new IndividuoNuevo(cromosomaOtroPadre, problemaOtro, generadorRandomOtroPadre);

            var ex = Assert.Throws<ArgumentException>(() => padre.Cruzar(otroPadre));
            Assert.Contains("no tienen la misma cantidad de cortes", ex.Message);
        }

        [Fact]
        public void Cruzar_CoincidenciasEnCerosYUnos_SeHeredan()
        {
            List<int> cromosomaPadreA = [1, 0, 1, 0, 1, 0];
            List<int> cromosomaPadreB = [1, 0, 0, 1, 0, 1];

            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();
            var generadorRandom = GeneradorNumerosRandomFactory.Crear(1);
            var padreA = new IndividuoNuevo(cromosomaPadreA, problema, generadorRandom);
            var padreB = new IndividuoNuevo(cromosomaPadreB, problema, generadorRandom);

            var hijo = padreA.Cruzar(padreB);

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
            var padreA = new IndividuoNuevo([1, 0, 1, 0, 1, 0], problema, generadorPadres);
            var padreB = new IndividuoNuevo([1, 0, 0, 1, 0, 1], problema, generadorPadres);

            var hijo = padreA.Cruzar(padreB);

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

            InstanciaProblema problema = CrearInstanciaProblemaCincoAtomosTresAgentes();
            var padre = new IndividuoNuevo([1, 0, 1, 0], problema, generador);

            var hijo = padre.Cruzar(padre);

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

            InstanciaProblema problema = CrearInstanciaProblemaSieteAtomosCuatroAgentes();
            var padreA = new IndividuoNuevo([1, 1, 0, 1, 0, 0], problema, generadorPadres);
            var padreB = new IndividuoNuevo([1, 0, 1, 0, 1, 0], problema, generadorPadres);

            var hijo = padreA.Cruzar(padreB);

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
            var generador = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo([1, 0, 1, 0], problema, generador);

            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 1, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_VecinasEmpatadas_AgrandaLaIzquierda()
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
            var individuo = new IndividuoNuevo([1, 0, 1, 0], problema, generador);

            individuo.Mutar();

            List<int> cromosomaEsperado = [0, 1, 1, 0];
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
            var generador = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo([1, 0, 1, 0], problema, generador);

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
            var generador = GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo([1, 0, 1, 0], problema, generador);

            individuo.Mutar();

            List<int> cromosomaEsperado = [1, 1, 0, 0];
            Assert.Equal(cromosomaEsperado, individuo.Cromosoma);
        }

        /*
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
            algoritmoHungaro.CalcularAsignacionOptimaDePorciones(Arg.Any<decimal[,]>()).Returns([0, 1, 2]);
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

        private IndividuoNuevo CrearIndividuo(
            List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom = null)
        {
            var generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
            var individuo = new IndividuoNuevo(cromosoma, problema, generador);
            return individuo;
        }
        */

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
    }
}
