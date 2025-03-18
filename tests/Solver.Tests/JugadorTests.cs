namespace Solver.Tests
{
    public class JugadorTests
    {
        [Fact]
        public void Constructor_Id_SeAsigna()
        {
            int id = 1;
            var jugador = new Jugador(id);
            Assert.Equal(id, jugador.Id);
        }

        [Fact]
        public void Constructor_Valoraciones_SeInicializaVacia()
        {
            var jugador = new Jugador(1);
            Assert.NotNull(jugador.Valoraciones);
            Assert.Empty(jugador.Valoraciones);
        }

        [Fact]
        public void AgregarValoracion_Valoracion_SeAgrega()
        {
            var atomo1 = new Atomo(1, 1);
            var atomo2 = new Atomo(2, 1);

            var jugador = new Jugador(1);
            jugador.AgregarValoracion(atomo1);
            jugador.AgregarValoracion(atomo2);

            Assert.True(jugador.Valoraciones.Count == 2);
            Assert.Same(atomo1, jugador.Valoraciones[0]);
            Assert.Same(atomo2, jugador.Valoraciones[1]);
        }

        [Fact]
        public void AgregarValoracion_ValoracionSobreMismoAtomo_LanzaExcepcion()
        {
            int posicionAtomo = 1;

            var jugador = new Jugador(1);
            jugador.AgregarValoracion(new Atomo(posicionAtomo, 1));

            var ex = Assert.Throws<InvalidOperationException>(() => jugador.AgregarValoracion(new Atomo(posicionAtomo, 0)));
            Assert.Equal($"Ya existe una valoración para el átomo #{posicionAtomo}", ex.Message);
        }
    }
}
