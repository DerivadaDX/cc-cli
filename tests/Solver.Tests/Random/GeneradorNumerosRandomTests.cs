using Solver.Random;

namespace Solver.Tests.Random
{
    public class GeneradorNumerosRandomTests
    {
        [Fact]
        public void GenerarNumerosRandom_SemillaNegativa_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => new GeneradorNumerosRandom(-1));
            Assert.StartsWith("La semilla no puede ser negativa: -1", ex.Message);
        }

        [Fact]
        public void Siguiente_ConSemillaFija_DevuelveMismosNumeros()
        {
            var generador1 = new GeneradorNumerosRandom(123);
            var generador2 = new GeneradorNumerosRandom(123);

            int numero1 = generador1.Siguiente();
            int numero2 = generador2.Siguiente();

            Assert.Equal(numero1, numero2);
        }

        [Fact]
        public void Siguiente_ConSemillaDiferente_DevuelveNumerosDiferentes()
        {
            var generador1 = new GeneradorNumerosRandom(123);
            var generador2 = new GeneradorNumerosRandom(456);

            int numero1 = generador1.Siguiente();
            int numero2 = generador2.Siguiente();

            Assert.NotEqual(numero1, numero2);
        }

        [Fact]
        public void Siguiente_ConRango_DevuelveNumeroEnRango()
        {
            var generador = new GeneradorNumerosRandom();
            int numero = generador.Siguiente(1, 100);
            Assert.InRange(numero, 1, 99);
        }
    }
}
