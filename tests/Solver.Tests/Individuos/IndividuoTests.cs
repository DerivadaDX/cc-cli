using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoTests
    {
        [Fact]
        public void Constructor_CromosomaNull_LanzaArgumentNullException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoFake(null, instanciaProblema));
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoFake([], null));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaVacio_LanzaArgumentException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([], instanciaProblema));
            Assert.Contains("no puede estar vacío", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaArgumentException()
        {
            // Para k agentes se esperan k-1 cortes y k asignaciones
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([1, 2], instanciaProblema));
            Assert.Contains("Cantidad de genes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([-1, 2, 1], instanciaProblema));
            Assert.Contains("primer corte no puede ser negativo", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([3, 2, 1], instanciaProblema));
            Assert.Contains("no puede superar la cantidad de átomos", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            // El rango permitido para las asignaciones de k agentes es [1, k]
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, 1, 0], instanciaProblema));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayMasDeUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, -1, 5], instanciaProblema));
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
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([1, 2, 3, 5, 2, 0, -1], instanciaProblema));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, 1, 1], instanciaProblema));
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
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoFake([0, 0, 4, 2, 2, 3, 3], instanciaProblema));
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
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });

            var ex = Record.Exception(() => new IndividuoFake(cromosomaValido, instanciaProblema));
            Assert.Null(ex);
        }

        [Fact]
        public void Constructor_CromosomaValidoParaUnAgente_NoLanzaExcepciones()
        {
            // 1ra porción se asigna al agente 1
            var cromosomaValido = new List<int> { 1 };

            // 1 átomo, 1 agente
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });

            var ex = Record.Exception(() => new IndividuoFake(cromosomaValido, instanciaProblema));
            Assert.Null(ex);
        }

        [Fact]
        public void ToString_DevuelveCromosomaYFitness()
        {
            var cromosoma = new List<int> { 1, 2, 1 };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });

            var individuo = new IndividuoFake(cromosoma, instanciaProblema);
            string resultado = individuo.ToString();

            Assert.Equal("Cromosoma: [1, 2, 1], Fitness: 1.0", resultado);
        }

        private class IndividuoFake : Individuo
        {
            internal IndividuoFake(List<int> cromosoma, InstanciaProblema problema)
                : base(cromosoma, problema, new CalculadoraFitness())
            {
            }

            internal override decimal Fitness()
            {
                return 1.0m;
            }

            internal override void Mutar()
            {
                throw new NotImplementedException();
            }

            internal override Individuo Cruzar(Individuo otro)
            {
                throw new NotImplementedException();
            }
        }
    }
}
