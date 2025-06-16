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
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).ForegroundColor(ConsoleColor.White);
        }

        [Fact]
        public void MostrarInfo_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).WriteLine("Mensaje de prueba");
        }

        [Fact]
        public void MostrarInfo_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo("Mensaje de prueba");

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarInfo_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo("Mensaje de prueba");

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarProgreso_Color_EsBlanco()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso("Mensaje de prueba");

            consola.Received(1).ForegroundColor(ConsoleColor.White);
        }

        [Fact]
        public void MostrarProgreso_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso("Mensaje de prueba");

            consola.Received(1).Write("\rMensaje de prueba");
        }

        [Fact]
        public void MostrarProgreso_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso("Mensaje de prueba");

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarProgreso_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso("Mensaje de prueba");

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.Write(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarAdvertencia_Color_EsAmarillo()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia("Mensaje de prueba");

            consola.Received(1).ForegroundColor(ConsoleColor.Yellow);
        }

        [Fact]
        public void MostrarAdvertencia_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia("Mensaje de prueba");

            consola.Received(1).WriteLine("Mensaje de prueba");
        }

        [Fact]
        public void MostrarAdvertencia_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia("Mensaje de prueba");

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarAdvertencia_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia("Mensaje de prueba");

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        private (Presentador presentador, ConsoleProxy consola) CrearPresentadorConConsolaFake()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);
            return (presentador, consola);
        }
    }
}
