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
    }
}
