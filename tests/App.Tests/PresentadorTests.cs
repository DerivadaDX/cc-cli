using NSubstitute;

namespace App.Tests
{
    public class PresentadorTests
    {
        [Fact]
        public void Constructor_ConsoleProxyNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Presentador(null));
            Assert.Equal("consola", ex.ParamName);
        }

        [Fact]
        public void MostrarInfo_Color_EsBlanco()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).ForegroundColor(ConsoleColor.White);
        }

        [Fact]
        public void MostrarInfo_Mensaje_SeEscribe()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).WriteLine("Mensaje de prueba");
        }

        [Fact]
        public void MostrarInfo_Color_SeResetea()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarInfo_OrdenLlamadas_EsCorrecto()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);

            presentador.MostrarInfo("Mensaje de prueba");

            Received.InOrder(() =>
            {
                consola.ForegroundColor(ConsoleColor.White);
                consola.WriteLine("Mensaje de prueba");
                consola.ResetColor();
            });
        }
    }
}
