namespace App.Tests
{
    public class ConsoleProxyFactoryTests : IDisposable
    {
        public void Dispose()
        {
            ConsoleProxyFactory.SetearConsola(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            var consola = ConsoleProxyFactory.Crear();
            Assert.NotNull(consola);
            Assert.IsType<ConsoleProxy>(consola);
        }

        [Fact]
        public void SetearProxy_Helper_SeSeteaCorrectamente()
        {
            var instancia = new ConsoleProxy();
            ConsoleProxyFactory.SetearConsola(instancia);

            var consola = ConsoleProxyFactory.Crear();

            Assert.Same(instancia, consola);
        }
    }
}
