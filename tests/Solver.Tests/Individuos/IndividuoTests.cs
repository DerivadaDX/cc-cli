using Solver.Fitness;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoTests
    {
        [Fact]
        public void Constructor_CromosomaNull_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub(null, new InstanciaProblema()));
            Assert.StartsWith("El cromosoma no puede ser null", ex.Message);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([], null));
            Assert.StartsWith("La instancia del problema no puede ser null", ex.Message);
        }

        [Fact]
        public void Constructor_CromosomaVacio_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([], new InstanciaProblema()));
            Assert.Equal("El cromosoma no puede estar vacío", ex.Message);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaExcepcion()
        {
            // Para k jugadores se esperan k-1 cortes y k asignaciones
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2], instanciaProblema));
            Assert.StartsWith("Cantidad de genes inválida. Esperada: 3, recibida: 2", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([-1, 2, 1], instanciaProblema));
            Assert.StartsWith($"Posición del primer corte no puede ser negativa: -1", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([3, 2, 1], instanciaProblema));
            Assert.StartsWith("Posición del último corte no puede superar a 2: 3", ex.Message);
        }

        [Fact]
        public void Constructor_HayUnaAsignacionAJugadoresInvalidoEnCromosoma_LanzaExcepcion()
        {
            // El rango permitido para las asignaciones de k jugadores es [1, k]
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 0], instanciaProblema));
            Assert.StartsWith($"Hay asignaciones fuera del rango [1, 2]: (0)", ex.Message);
        }

        [Fact]
        public void Constructor_HayMasDeUnaAsignacionAJugadoresInvalidosEnCromosoma_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, -1, 5], instanciaProblema));
            Assert.StartsWith("Hay asignaciones fuera del rango [1, 2]: (-1, 5)", ex.Message);
        }

        [Fact]
        public void Constructor_ListadoDeAsignacionesRepetidasSeMuestraOrdenadoAscendente_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([
                [1m, 0m, 0m, 0m],
                [0m, 1m, 0m, 0m],
                [0m, 0m, 1m, 0m],
                [0m, 0m, 0m, 1m],
            ]);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2, 3, 5, 2, 0, -1], instanciaProblema));
            Assert.StartsWith("Hay asignaciones fuera del rango [1, 4]: (-1, 0, 5)", ex.Message);
        }

        [Fact]
        public void Constructor_HayUnaPorcionAsignadaAMasDeUnJugadorEnCromosoma_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 1], instanciaProblema));
            Assert.StartsWith("Hay porciones asignadas a más de un jugador: (1)", ex.Message);
        }

        [Fact]
        public void Constructor_HayMasDeUnaPorcionAsignadaAMasDeUnJugadorEnCromosoma_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([
                [1m, 0m, 0m, 0m],
                [0m, 1m, 0m, 0m],
                [0m, 0m, 1m, 0m],
                [0m, 0m, 0m, 1m],
             ]);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 0, 4, 2, 2, 3, 3], instanciaProblema));
            Assert.StartsWith("Hay porciones asignadas a más de un jugador: (2, 3)", ex.Message);
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
