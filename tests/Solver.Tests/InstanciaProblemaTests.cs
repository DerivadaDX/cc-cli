namespace Solver.Tests
{
    public class InstanciaProblemaTests
    {
        [Fact]
        public void Constructor_Jugadores_SeInicializaVacio()
        {
            var instancia = new InstanciaProblema();
            Assert.NotNull(instancia.Jugadores);
            Assert.Empty(instancia.Jugadores);
        }
    }
}
