﻿using NSubstitute;

namespace App.Tests
{
    public class PresentadorTests
    {
        private const string MensajeDePrueba = "Mensaje de prueba";

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

            presentador.MostrarInfo(MensajeDePrueba);

            consola.Received(1).ForegroundColor(ConsoleColor.White);
        }

        [Fact]
        public void MostrarInfo_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo(MensajeDePrueba);

            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarInfo_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo(MensajeDePrueba);

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarInfo_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo(MensajeDePrueba);

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarInfo_SinLineaEnCurso_NoAgregaSaltosDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarInfo(MensajeDePrueba);

            consola.DidNotReceive().WriteLine(string.Empty);
        }

        [Fact]
        public void MostrarInfo_LineaEnCurso_AgregaSaltoDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);
            presentador.MostrarInfo(MensajeDePrueba);

            consola.Received(1).WriteLine(string.Empty);
            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarExito_Color_EsVerde()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarExito(MensajeDePrueba);

            consola.Received(1).ForegroundColor(ConsoleColor.Green);
        }

        [Fact]
        public void MostrarExito_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarExito(MensajeDePrueba);

            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarExito_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarExito(MensajeDePrueba);

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarExito_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarExito(MensajeDePrueba);

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarExito_SinLineaEnCurso_NoAgregaSaltosDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarExito(MensajeDePrueba);

            consola.DidNotReceive().WriteLine(string.Empty);
        }

        [Fact]
        public void MostrarExito_LineaEnCurso_AgregaSaltoDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);
            presentador.MostrarExito(MensajeDePrueba);

            consola.Received(1).WriteLine(string.Empty);
            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarAdvertencia_Color_EsAmarillo()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia(MensajeDePrueba);

            consola.Received(1).ForegroundColor(ConsoleColor.Yellow);
        }

        [Fact]
        public void MostrarAdvertencia_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia(MensajeDePrueba);

            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarAdvertencia_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia(MensajeDePrueba);

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarAdvertencia_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia(MensajeDePrueba);

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarAdvertencia_SinLineaEnCurso_NoAgregaSaltosDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarAdvertencia(MensajeDePrueba);

            consola.DidNotReceive().WriteLine(string.Empty);
        }

        [Fact]
        public void MostrarAdvertencia_LineaEnCurso_AgregaSaltoDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);
            presentador.MostrarAdvertencia(MensajeDePrueba);

            consola.Received(1).WriteLine(string.Empty);
            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarError_Color_EsRojo()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarError(MensajeDePrueba);

            consola.Received(1).ForegroundColor(ConsoleColor.Red);
        }

        [Fact]
        public void MostrarError_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarError(MensajeDePrueba);

            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarError_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarError(MensajeDePrueba);

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarError_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarError(MensajeDePrueba);

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.WriteLine(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarError_SinLineaEnCurso_NoAgregaSaltosDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarError(MensajeDePrueba);

            consola.DidNotReceive().WriteLine(string.Empty);
        }

        [Fact]
        public void MostrarError_LineaEnCurso_AgregaSaltoDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);
            presentador.MostrarError(MensajeDePrueba);

            consola.Received(1).WriteLine(string.Empty);
            consola.Received(1).WriteLine(MensajeDePrueba);
        }

        [Fact]
        public void MostrarProgreso_Color_EsBlanco()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);

            consola.Received(1).ForegroundColor(ConsoleColor.White);
        }

        [Fact]
        public void MostrarProgreso_Mensaje_SeEscribe()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);

            consola.Received(1).Write("\rMensaje de prueba");
        }

        [Fact]
        public void MostrarProgreso_Color_SeResetea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);

            consola.Received(1).ResetColor();
        }

        [Fact]
        public void MostrarProgreso_OrdenLlamadas_EsCorrecto()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);

            Received.InOrder(() =>
            {
                consola.ForegroundColor(Arg.Any<ConsoleColor>());
                consola.Write(Arg.Any<string>());
                consola.ResetColor();
            });
        }

        [Fact]
        public void MostrarProgreso_LlamadoConsecutivo_NoAgregaSaltoDeLinea()
        {
            (Presentador presentador, ConsoleProxy consola) = CrearPresentadorConConsolaFake();

            presentador.MostrarProgreso(MensajeDePrueba);
            presentador.MostrarProgreso(MensajeDePrueba);

            consola.DidNotReceive().WriteLine(string.Empty);
        }

        private (Presentador presentador, ConsoleProxy consola) CrearPresentadorConConsolaFake()
        {
            var consola = Substitute.For<ConsoleProxy>();
            var presentador = new Presentador(consola);
            return (presentador, consola);
        }
    }
}
