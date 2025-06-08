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
        public void CrearInicial_IndividuoFactory_Null_Excepcion()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => PoblacionFactory.CrearInicial(5, null));
            Assert.Equal("individuoFactory", ex.ParamName);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            int tamaño = 5;

            var poblacion = PoblacionFactory.CrearInicial(tamaño, Substitute.For<IIndividuoFactory>());

            Assert.NotNull(poblacion);
            Assert.IsType<Poblacion>(poblacion);
            Assert.Equal(tamaño, poblacion.Individuos.Count);
        }

        [Fact]
        public void SetearPoblacion_Poblacion_SeSeteaCorrectamente()
        {
            var instancia1 = new Poblacion(1);
            PoblacionFactory.SetearPoblacion(instancia1);

            var instancia2 = PoblacionFactory.CrearInicial(1, Substitute.For<IIndividuoFactory>());

            Assert.Same(instancia1, instancia2);
        }
    }
}
