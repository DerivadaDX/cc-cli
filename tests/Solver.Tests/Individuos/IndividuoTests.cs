using Solver.Fitness;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoTests
    {
        [Fact]
        public void Constructor_CromosomaNull_LanzaArgumentNullException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoStub(null, instanciaProblema));
            Assert.Contains("cromosoma", ex.Message);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoStub([], null));
            Assert.Contains("problema", ex.Message);
        }

        [Fact]
        public void Constructor_CromosomaVacio_LanzaArgumentException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([], instanciaProblema));
            Assert.StartsWith("El cromosoma no puede estar vacío", ex.Message);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaArgumentException()
        {
            // Para k agentes se esperan k-1 cortes y k asignaciones
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2], instanciaProblema));
            Assert.StartsWith("Cantidad de genes inválida. Esperada: 3, recibida: 2", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([-1, 2, 1], instanciaProblema));
            Assert.StartsWith($"Posición del primer corte no puede ser negativa: -1", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([3, 2, 1], instanciaProblema));
            Assert.StartsWith("Posición del último corte no puede superar a 2: 3", ex.Message);
        }

        [Fact]
        public void Constructor_HayUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            // El rango permitido para las asignaciones de k agentes es [1, k]
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 0], instanciaProblema));
            Assert.StartsWith($"Hay asignaciones fuera del rango [1, 2]: (0)", ex.Message);
        }

        [Fact]
        public void Constructor_HayMasDeUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, -1, 5], instanciaProblema));
            Assert.StartsWith("Hay asignaciones fuera del rango [1, 2]: (-1, 5)", ex.Message);
        }

        [Fact]
        public void Constructor_ListadoDeAsignacionesRepetidasSeMuestraOrdenadoAscendente_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,]
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2, 3, 5, 2, 0, -1], instanciaProblema));
            Assert.StartsWith("Hay asignaciones fuera del rango [1, 4]: (-1, 0, 5)", ex.Message);
        }

        [Fact]
        public void Constructor_HayUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,] { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 1], instanciaProblema));
            Assert.StartsWith("Hay porciones asignadas a más de un agente: (1)", ex.Message);
        }

        [Fact]
        public void Constructor_HayMasDeUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = new decimal[,]
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 0, 4, 2, 2, 3, 3], instanciaProblema));
            Assert.StartsWith("Hay porciones asignadas a más de un agente: (2, 3)", ex.Message);
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

            var ex = Record.Exception(() => new IndividuoStub(cromosomaValido, instanciaProblema));
            Assert.Null(ex);
        }

        private class IndividuoStub : Individuo
        {
            internal IndividuoStub(List<int> cromosoma, InstanciaProblema problema) : base(cromosoma, problema)
            {
            }

            internal override void Mutar()
            {
                throw new NotImplementedException();
            }

            internal override Individuo Cruzar(Individuo otro)
            {
                throw new NotImplementedException();
            }

            internal override void CalcularFitness(ICalculadoraFitness calculadoraFitness)
            {
                throw new NotImplementedException();
            }
        }
    }
}
