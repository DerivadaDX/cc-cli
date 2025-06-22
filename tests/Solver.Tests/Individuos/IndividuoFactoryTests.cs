using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoFactoryTests
    {
        [Fact]
        public void CrearAleatorio_ProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => IndividuoFactory.CrearAleatorio(null, TipoIndividuo.IntercambioAsignaciones));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void CrearAleatorio_TipoIntercambioAsignaciones_DevuelveIndividuoIntercambio()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.IntercambioAsignaciones);
            Assert.IsType<IndividuoIntercambioAsignaciones>(individuo);
        }

        [Fact]
        public void CrearAleatorio_TipoOptimizacionAsignaciones_DevuelveIndividuoOptimizacion()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.OptimizacionAsignaciones);
            Assert.IsType<IndividuoOptimizacionAsignaciones>(individuo);
        }

        [Fact]
        public void CrearAleatorio_Cromosoma_TieneLongitudEsperada()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
            var individuo = IndividuoFactory.CrearAleatorio(problema, TipoIndividuo.IntercambioAsignaciones);

            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortes = cantidadAgentes - 1;
            int longitudEsperada = cantidadCortes + cantidadAgentes;
            Assert.Equal(longitudEsperada, individuo.Cromosoma.Count);
        }
    }
}
