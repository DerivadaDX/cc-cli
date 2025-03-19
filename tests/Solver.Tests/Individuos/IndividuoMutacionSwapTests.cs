using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoMutacionSwapTests
    {
        [Fact]
        public void Constructor_CromosomaVacio_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoMutacionSwap([], new InstanciaProblema()));
            Assert.Equal("El cromosoma no puede estar vacío", ex.Message);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaExcepcion()
        {
            // Para k jugadores se esperan k-1 cortes y k asignaciones
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoMutacionSwap([1, 2], instanciaProblema));
            Assert.StartsWith("Cantidad de genes inválida. Esperada: 3, recibida: 2", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoMutacionSwap([-1, 2, 1], instanciaProblema));
            Assert.StartsWith($"Posición del primer corte no puede ser negativa: -1", ex.Message);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaExcepcion()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1m, 0m], [0m, 1m]]);
            var ex = Assert.Throws<ArgumentException>(() => new IndividuoMutacionSwap([3, 2, 1], instanciaProblema));
            Assert.StartsWith("Posición del último corte no puede superar a 2: 3", ex.Message);
        }
    }
}
