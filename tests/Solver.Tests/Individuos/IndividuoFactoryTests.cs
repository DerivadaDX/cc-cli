using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoFactoryTests
    {
        [Fact]
        public void CrearAleatorio_ProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                IndividuoFactory.CrearAleatorio(
                    null,
                    TipoIndividuo.IntercambioAsignaciones,
                    Substitute.For<GeneradorNumerosRandom>(1)
                )
            );
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void CrearAleatorio_GeneradorRandomNull_LanzaArgumentNullException()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var ex = Assert.Throws<ArgumentNullException>(() =>
                IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.IntercambioAsignaciones, null)
            );
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void CrearAleatorio_TipoIntercambioAsignaciones_DevuelveIndividuoIntercambio()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.IntercambioAsignaciones, generadorRandom);
            Assert.IsType<IndividuoIntercambioAsignaciones>(individuo);
        }

        [Fact]
        public void CrearAleatorio_TipoOptimizacionAsignaciones_DevuelveIndividuoOptimizacion()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.OptimizacionAsignaciones, generadorRandom);
            Assert.IsType<IndividuoOptimizacionAsignaciones>(individuo);
        }

        [Fact]
        public void CrearAleatorio_Cromosoma_TieneLongitudEsperada()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.IntercambioAsignaciones, generadorRandom);

            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortes = cantidadAgentes - 1;
            int longitudEsperada = cantidadCortes + cantidadAgentes;
            Assert.Equal(longitudEsperada, individuo.Cromosoma.Count);
        }
    }
}
