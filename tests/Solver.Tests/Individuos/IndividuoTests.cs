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
                    { 1m },
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
                    { 1m },
                }
            );

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoFake([], instanciaProblema, null!));
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        private class IndividuoFake : Individuo
        {
            internal IndividuoFake(List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
                : base(cromosoma, problema, generadorRandom) { }

            protected override string FamiliaCromosoma => "fake";

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
