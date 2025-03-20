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
        public void CrearDesdeMatrizDeValoraciones_MatrizNull_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(null));
            Assert.StartsWith("La matriz de valoraciones no puede ser null", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizConFilasDesiguales_LanzaExcepcion()
        {
            decimal[][] matriz = [
                [0m, 0m, 1m],
                [0.5m, 0.5m],
            ];
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz));
            Assert.StartsWith("Todas las filas de la matriz deben tener la misma longitud", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizVacia_CreaInstanciaVacia()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([]);
            Assert.NotNull(instanciaProblema.Jugadores);
            Assert.Empty(instanciaProblema.Jugadores);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoracione_MatrizValida_CreaInstanciaCorrectamente()
        {
            var instancia = InstanciaProblema.CrearDesdeMatrizDeValoraciones([
                [ 0.1m, 0.2m, 0m, 0m, 0m, 0.6m, 0m ],
                [ 0m, 0m, 0.3m, 0.3m, 0.3m, 0m, 0.1m ],
            ]);

            Assert.Equal(2, instancia.Jugadores.Count);
            Assert.Equal(7, instancia.CantidadAtomos);

            Jugador jugador1 = instancia.Jugadores[0];
            Assert.Equal(1, jugador1.Id);
            Assert.Equal(3, jugador1.Valoraciones.Count);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 1 && a.Valoracion == 0.1m);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 2 && a.Valoracion == 0.2m);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 6 && a.Valoracion == 0.6m);

            Jugador jugador2 = instancia.Jugadores[1];
            Assert.Equal(2, jugador2.Id);
            Assert.Equal(4, jugador2.Valoraciones.Count);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 3 && a.Valoracion == 0.3m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 4 && a.Valoracion == 0.3m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 5 && a.Valoracion == 0.3m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 7 && a.Valoracion == 0.1m);
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

        [Fact]
        public void CantidadAtomos_JugadoresSinValoraciones_CantidadEsCero()
        {
            var instanciaProblema = new InstanciaProblema();
            instanciaProblema.AgregarJugador(new Jugador(1));
            instanciaProblema.AgregarJugador(new Jugador(2));
            Assert.Equal(0, instanciaProblema.CantidadAtomos);
        }

        [Fact]
        public void CantidadAtomos_SeAgreganValoraciones_CantidadEsCorrecta()
        {
            var jugador1 = new Jugador(1);
            var jugador2 = new Jugador(2);
            jugador1.AgregarValoracion(new Atomo(1, 1));
            jugador1.AgregarValoracion(new Atomo(3, 1));
            jugador2.AgregarValoracion(new Atomo(5, 1));

            var instanciaProblema = new InstanciaProblema();
            instanciaProblema.AgregarJugador(jugador1);
            instanciaProblema.AgregarJugador(jugador2);

            Assert.Equal(3, instanciaProblema.CantidadAtomos);
        }
    }
}
