namespace Common.Tests
{
    public class GeneradorNumerosRandomTests
    {
        [Fact]
        public void Constructor_SemillaNegativa_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new GeneradorNumerosRandom(-1));
            Assert.Contains("no puede ser negativa", ex.Message);
            Assert.Equal("seed", ex.ParamName);
        }

        [Fact]
        public void SetearSeed_SemillaNegativa_LanzaArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => GeneradorNumerosRandom.SetearSeed(-1));
            Assert.Contains("no puede ser negativa", ex.Message);
            Assert.Equal("seed", ex.ParamName);
        }

        [Fact]
        public void GenerarSeed_SinSetearSeed_DevuelveValorNoNegativo()
        {
            int seed = GeneradorNumerosRandom.GenerarSeed();
            Assert.True(seed >= 0);
        }

        [Fact]
        public void GenerarSeed_ConSeedSeteada_DevuelveMismoValor()
        {
            int seedCustom = 123;
            GeneradorNumerosRandom.SetearSeed(seedCustom);

            int seed1 = GeneradorNumerosRandom.GenerarSeed();
            int seed2 = GeneradorNumerosRandom.GenerarSeed();

            Assert.Equal(seedCustom, seed1);
            Assert.Equal(seedCustom, seed2);
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

        [Fact]
        public void Siguiente_ConMaximo_DevuelveNumeroEnRango()
        {
            var generador = new GeneradorNumerosRandom();
            int numero = generador.Siguiente(100);
            Assert.InRange(numero, 0, 99);
        }

        [Fact]
        public void SiguienteDouble_ConSemillaFija_DevuelveMismosValores()
        {
            var generador1 = new GeneradorNumerosRandom(123);
            var generador2 = new GeneradorNumerosRandom(123);

            double valor1 = generador1.SiguienteDouble();
            double valor2 = generador2.SiguienteDouble();

            Assert.Equal(valor1, valor2);
        }

        [Fact]
        public void SiguienteDouble_ValorDevuelto_EntreCeroYUno()
        {
            var generador = new GeneradorNumerosRandom();
            double valor = generador.SiguienteDouble();
            Assert.InRange(valor, 0, 1);
        }
    }
}
