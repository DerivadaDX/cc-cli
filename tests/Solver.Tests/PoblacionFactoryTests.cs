using Common;
using NSubstitute;
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
            var ex = Assert.Throws<ArgumentNullException>(() =>
                PoblacionFactory.Crear(5, null, TipoIndividuo.IntercambioAsignaciones, Substitute.For<GeneradorNumerosRandom>(1))
            );
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Crear_GeneradorRandomNull_LanzaArgumentNullException()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );

            var ex = Assert.Throws<ArgumentNullException>(() =>
                PoblacionFactory.Crear(5, problema, TipoIndividuo.IntercambioAsignaciones, null)
            );
            Assert.Equal("generadorRandom", ex.ParamName);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            int tamaño = 5;
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );

            var poblacion = PoblacionFactory.Crear(
                tamaño,
                problema,
                TipoIndividuo.IntercambioAsignaciones,
                Substitute.For<GeneradorNumerosRandom>(1)
            );

            Assert.NotNull(poblacion);
            Assert.IsType<Poblacion>(poblacion);
            Assert.Equal(tamaño, poblacion.Individuos.Count);
        }

        [Fact]
        public void SetearPoblacion_Poblacion_SeSeteaCorrectamente()
        {
            var poblacionSeteada = new Poblacion(1, Substitute.For<GeneradorNumerosRandom>(1));
            PoblacionFactory.SetearPoblacion(poblacionSeteada);

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var poblacionObtenida = PoblacionFactory.Crear(
                1,
                problema,
                TipoIndividuo.IntercambioAsignaciones,
                Substitute.For<GeneradorNumerosRandom>(1)
            );

            Assert.Same(poblacionSeteada, poblacionObtenida);
        }
    }
}
