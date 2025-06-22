using Solver.Individuos;

namespace Solver.Tests
{
    public class PoblacionFactoryTests : IDisposable
    {
        public void Dispose()
        {
            PoblacionFactory.SetearPoblacion(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_ProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => PoblacionFactory.Crear(5, null, TipoIndividuo.IntercambioAsignaciones));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            int tamaño = 5;
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });

            var poblacion = PoblacionFactory.Crear(tamaño, problema, TipoIndividuo.IntercambioAsignaciones);

            Assert.NotNull(poblacion);
            Assert.IsType<Poblacion>(poblacion);
            Assert.Equal(tamaño, poblacion.Individuos.Count);
        }

        [Fact]
        public void SetearPoblacion_Poblacion_SeSeteaCorrectamente()
        {
            var poblacionSeteada = new Poblacion(1);
            PoblacionFactory.SetearPoblacion(poblacionSeteada);

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
            var poblacionObtenida = PoblacionFactory.Crear(1, problema, TipoIndividuo.IntercambioAsignaciones);

            Assert.Same(poblacionSeteada, poblacionObtenida);
        }
    }
}
