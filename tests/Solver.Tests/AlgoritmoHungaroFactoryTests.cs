namespace Solver.Tests
{
    public class AlgoritmoHungaroFactoryTests : IDisposable
    {
        public void Dispose()
        {
            AlgoritmoHungaroFactory.SetearInstancia(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            var algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
            Assert.NotNull(algoritmoHungaro);
            Assert.IsType<AlgoritmoHungaro>(algoritmoHungaro);
        }

        [Fact]
        public void SetearInstancia_Instancia_SeSeteaCorrectamente()
        {
            var algoritmoHungaroSeteado = new AlgoritmoHungaro();
            AlgoritmoHungaroFactory.SetearInstancia(algoritmoHungaroSeteado);

            var algoritmoHungaroObtenido = AlgoritmoHungaroFactory.Crear();

            Assert.Same(algoritmoHungaroSeteado, algoritmoHungaroObtenido);
        }
    }
}
