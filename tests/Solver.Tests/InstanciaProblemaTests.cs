namespace Solver.Tests
{
    public class InstanciaProblemaTests
    {
        [Fact]
        public void Constructor_Jugadores_SeInicializaVacio()
        {
            var instanciaProblema = new InstanciaProblema();
            Assert.NotNull(instanciaProblema.Jugadores);
            Assert.Empty(instanciaProblema.Jugadores);
        }

        [Fact]
        public void AgregarJugador_JugadorRepetido_LanzaExcepcion()
        {
            var jugador1 = new Jugador(1);
            var jugador2 = new Jugador(1);

            var instanciaProblema = new InstanciaProblema();
            instanciaProblema.AgregarJugador(jugador1);

            var ex = Assert.Throws<InvalidOperationException>(() => instanciaProblema.AgregarJugador(jugador2));
            Assert.Equal("Ya existe un jugador con el id 1", ex.Message);
        }

        [Fact]
        public void AgregarJugador_JugadorValoraUnAtomoQueYaFueValorado_LanzaExcepcion()
        {
            var atomo = new Atomo(1, 1);
            var jugador1 = new Jugador(1);
            var jugador2 = new Jugador(2);
            jugador1.AgregarValoracion(atomo);
            jugador2.AgregarValoracion(atomo);

            var instanciaProblema = new InstanciaProblema();
            instanciaProblema.AgregarJugador(jugador1);

            var ex = Assert.Throws<InvalidOperationException>(() => instanciaProblema.AgregarJugador(jugador2));
            Assert.Equal($"El jugador #{jugador2.Id} valora al átomo #{atomo.Posicion} que ya fue valorado por otro", ex.Message);
        }

        [Fact]
        public void AgregarJugador_Jugador_SeAgregaALista()
        {
            var jugador1 = new Jugador(1);
            var jugador2 = new Jugador(2);

            var instanciaProblema = new InstanciaProblema();
            instanciaProblema.AgregarJugador(jugador1);
            instanciaProblema.AgregarJugador(jugador2);

            Assert.True(instanciaProblema.Jugadores.Count == 2);
            Assert.Same(jugador1, instanciaProblema.Jugadores[0]);
            Assert.Same(jugador2, instanciaProblema.Jugadores[1]);
        }
    }
}
