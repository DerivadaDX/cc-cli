namespace Solver.Tests
{
    public class InstanciaProblemaTests
    {
        [Fact]
        public void Constructor_Jugadores_SeInicializaVacio()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([[1]]);
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
        public void CrearDesdeMatrizDeValoraciones_MatrizVacia_LanzaExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones([]));
            Assert.StartsWith("La matriz de valoraciones no puede estar vacía", ex.Message);
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
        public void CrearDesdeMatrizDeValoraciones_MatrizConJugadoresSinValoraciones_LanzaExcepcion()
        {
            decimal[][] matriz = [
                [0, 1, 1],
                [0, 1, 1],
            ];
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz));
            Assert.StartsWith("El jugador 1 no tiene valoraciones positivas sobre ningún átomo.", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizValida_CreaInstanciaCorrectamente()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([
                [0, 3.9m],
                [1, 1.2m],
                [1, 4.6m],
                [2, 1.5m],
                [3, 5.3m],
                [5, 0.0m],
            ]);

            Assert.Equal(2, instanciaProblema.Jugadores.Count);
            Assert.Equal(6, instanciaProblema.CantidadAtomos);

            Jugador jugador1 = instanciaProblema.Jugadores[0];
            Assert.Equal(1, jugador1.Id);
            Assert.Equal(5, jugador1.Valoraciones.Count);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 2 && a.Valoracion == 1);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 3 && a.Valoracion == 1);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 4 && a.Valoracion == 2);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 5 && a.Valoracion == 3);
            Assert.Contains(jugador1.Valoraciones, a => a.Posicion == 6 && a.Valoracion == 5);

            Jugador jugador2 = instanciaProblema.Jugadores[1];
            Assert.Equal(2, jugador2.Id);
            Assert.Equal(5, jugador2.Valoraciones.Count);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 1 && a.Valoracion == 3.9m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 2 && a.Valoracion == 1.2m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 3 && a.Valoracion == 4.6m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 4 && a.Valoracion == 1.5m);
            Assert.Contains(jugador2.Valoraciones, a => a.Posicion == 5 && a.Valoracion == 5.3m);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_CantidadAtomos_SeAsignaCorrectamente()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones([
                [1, 1],
                [1, 1],
            ]);

            Assert.Equal(2, instanciaProblema.CantidadAtomos);
        }
    }
}
