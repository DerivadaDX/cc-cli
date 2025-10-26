using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoTests
    {
        [Fact]
        public void Constructor_CromosomaNull_LanzaArgumentNullException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1 },
                }
            );

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new IndividuoFake(null!, instanciaProblema, Substitute.For<GeneradorNumerosRandom>(1))
            );
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new IndividuoFake([], null!, Substitute.For<GeneradorNumerosRandom>(1))
            );
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Constructor_GeneradorRandomNull_LanzaArgumentNullException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1 },
                }
            );

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoFake([], instanciaProblema, null!));
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaVacio_LanzaArgumentException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1 },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([], instanciaProblema, generadorRandom));
            Assert.Contains("no puede estar vacío", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaArgumentException()
        {
            // Para k agentes se esperan k-1 cortes y k asignaciones
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([1, 2], instanciaProblema, generadorRandom));
            Assert.Contains("Cantidad de genes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([-1, 2, 1], instanciaProblema, generadorRandom));
            Assert.Contains("primer corte no puede ser negativo", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([3, 2, 1], instanciaProblema, generadorRandom));
            Assert.Contains("no puede superar la cantidad de átomos", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            // El rango permitido para las asignaciones de k agentes es [1, k]
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, 1, 0], instanciaProblema, generadorRandom));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayMasDeUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, -1, 5], instanciaProblema, generadorRandom));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_ListadoDeAsignacionesRepetidasSeMuestraOrdenadoAscendente_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() =>
                new IndividuoFake([1, 2, 3, 5, 2, 0, -1], instanciaProblema, generadorRandom)
            );
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m },
                { 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, 1, 1], instanciaProblema, generadorRandom));
            Assert.Contains("asignaciones repetidas", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayMasDeUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m },
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Assert.Throws<ArgumentException>(() =>
                new IndividuoFake([0, 0, 4, 2, 2, 3, 3], instanciaProblema, generadorRandom)
            );
            Assert.Contains("asignaciones repetidas", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaValido_NoLanzaExcepciones()
        {
            // Cortes: 1, Asignaciones: 2, 1
            // - 1ra porción se asigna al agente 2
            // - 2da Porción se asigna al agente 1
            var cromosomaValido = new List<int> { 1, 2, 1 };

            // 3 átomos, 2 agentes
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                    { 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Record.Exception(() => new IndividuoFake(cromosomaValido, instanciaProblema, generadorRandom));
            Assert.Null(ex);
        }

        [Fact]
        public void Constructor_CromosomaValidoParaUnAgente_NoLanzaExcepciones()
        {
            // 1ra porción se asigna al agente 1
            var cromosomaValido = new List<int> { 1 };

            // 1 átomo, 1 agente
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var ex = Record.Exception(() => new IndividuoFake(cromosomaValido, instanciaProblema, generadorRandom));
            Assert.Null(ex);
        }

        [Fact]
        public void Mutar_CromosomaConUnSoloGen_NoMuta()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(0.0);

            var cromosomaOriginal = new List<int> { 1 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m },
                }
            );
            var individuo = new IndividuoFake([.. cromosomaOriginal], problema, generadorRandom);

            individuo.Mutar();

            Assert.Equal(cromosomaOriginal, individuo.Cromosoma);
        }

        [Fact]
        public void Mutar_Cromosoma_MutaCuandoProbabilidadLoPermite()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(0.0, 1.0); // Solo el primero muta
            generadorRandom.Siguiente(2).Returns(1); // Dirección +1

            var cromosomaOriginal = new List<int> { 0, 1, 2 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var individuo = new IndividuoFake([.. cromosomaOriginal], problema, generadorRandom);

            individuo.Mutar();

            Assert.NotEqual(cromosomaOriginal[0], individuo.Cromosoma[0]);
            Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[1]);
            Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[2]);
        }

        [Fact]
        public void Mutar_MutacionEnCorteSaleDeRangoPorIzquierda_ComportamientoCiclicoPorDerecha()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(0.0, 1.0); // Mutan los cortes, las asignaciones no
            generadorRandom.Siguiente(2).Returns(0);

            var cromosomaOriginal = new List<int> { 0, 1, 2 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var individuo = new IndividuoFake([.. cromosomaOriginal], problema, generadorRandom);

            individuo.Mutar();

            Assert.Equal(2, individuo.Cromosoma[0]);
        }

        [Fact]
        public void Mutar_MutacionEnCorteSaleDeRangoPorDerecha_ComportamientoCiclicoPorIzquierda()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(0.0, 1.0); // Mutan los cortes, las asignaciones no
            generadorRandom.Siguiente(2).Returns(1);

            var cromosomaOriginal = new List<int> { 2, 1, 2 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var individuo = new IndividuoFake([.. cromosomaOriginal], problema, generadorRandom);

            individuo.Mutar();

            Assert.Equal(0, individuo.Cromosoma[0]);
        }

        [Fact]
        public void Cruzar_PadresConDistintaCantidadDeCromosomas_LanzaExcepcion()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            var problema1 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema1, generadorRandom);

            var problema2 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m },
                }
            );
            var padre2 = new IndividuoFake([1], problema2, generadorRandom);

            var ex = Assert.Throws<ArgumentException>(() => padre1.Cruzar(padre2));
            Assert.Contains("misma cantidad de cromosomas", ex.Message);
        }

        [Fact]
        public void Cruzar_PadresConUnSoloCromosoma_CreaHijoConUnSoloCromosoma()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            var padre1 = new IndividuoFake([1], problema, generadorRandom);
            var padre2 = new IndividuoFake([1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Single(hijo.Cromosoma);
            Assert.Equal(1, hijo.Cromosoma[0]);
        }

        [Fact]
        public void Cruzar_SeccionDeCortesPuntoDeCruceAlInicio_QuedanCortesDePadre2()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(1, 3).Returns(0); // Punto de corte en la posición 0

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([3, 3, 3], hijo.Cromosoma.Take(3));
        }

        [Fact]
        public void Cruzar_SeccionDeCortesPuntoDeCruceAlMedio_PrimeraParteDePadre1RestoDePadre2()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(1, 3).Returns(1); // Punto de corte en la posición 1

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([1, 3, 3], hijo.Cromosoma.Take(3));
        }

        [Fact]
        public void Cruzar_SeccionDeCortesPuntoDeCruceAlFinal_QuedanCortesDePadre1()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(1, 3).Returns(2); // Punto de corte en la posición 2

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([1, 2, 3], hijo.Cromosoma.Take(3));
        }

        [Fact]
        public void Cruzar_SeccionDeAsignacionesSegmentoAlInicio_CompletaElFinal()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(4).Returns(0); // Inicio del segmento en la posición 0
            generadorRandom.Siguiente(0, 4).Returns(0); // Fin del segmento en la posición 0

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([1, 4, 3, 2], hijo.Cromosoma.Skip(3));
        }

        [Fact]
        public void Cruzar_SeccionDeAsignacionesSegmentoAlMedio_CompletaInicioYFinal()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(4).Returns(1); // Inicio del segmento en la posición 1
            generadorRandom.Siguiente(1, 4).Returns(2); // Fin del segmento en la posición 2

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
        }

        [Fact]
        public void Cruzar_SeccionDeAsignacionesSegmentoAlFinal_CompletaElInicio()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(4).Returns(3); // Inicio del segmento en la posición 3
            generadorRandom.Siguiente(3, 4).Returns(3); // Fin del segmento en la posición 3

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([3, 2, 1, 4], hijo.Cromosoma.Skip(3));
        }

        [Fact]
        public void Cruzar_SeccionDeAsignacionesSegmentoCompleto_QuedaTodoPadre1()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.Siguiente(4).Returns(0); // Inicio del segmento en la posición 0
            generadorRandom.Siguiente(0, 4).Returns(3); // Fin del segmento en la posición 3

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var padre1 = new IndividuoFake([1, 2, 3, 1, 2, 3, 4], problema, generadorRandom);
            var padre2 = new IndividuoFake([3, 3, 3, 4, 3, 2, 1], problema, generadorRandom);

            Individuo hijo = padre1.Cruzar(padre2);

            Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
        }

        [Fact]
        public void ToString_Resultado_CromosomaCortesOrdenadoYFitness()
        {
            var cromosoma = new List<int> { 4, 1, 2, 1, 4, 3, 2 };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m, 0m, 0m },
                    { 0m, 1m, 0m, 0m },
                    { 0m, 0m, 1m, 0m },
                    { 0m, 0m, 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            var individuo = new IndividuoFake(cromosoma, instanciaProblema, generadorRandom);
            string resultado = individuo.ToString();

            Assert.Equal("Cromosoma=[1, 2, 4, 1, 4, 3, 2], Fitness=1.0", resultado);
        }

        private class IndividuoFake : Individuo
        {
            internal IndividuoFake(List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
                : base(cromosoma, problema, generadorRandom) { }

            internal override decimal Fitness()
            {
                return 1.0m;
            }

            protected override void MutarAsignaciones() { }

            protected override Individuo CrearNuevoIndividuo(List<int> cromosoma)
            {
                var individuo = new IndividuoFake(cromosoma, _problema, _generadorRandom);
                return individuo;
            }
        }
    }
}
