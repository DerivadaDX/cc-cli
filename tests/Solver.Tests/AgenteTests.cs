namespace Solver.Tests
{
    public class AgenteTests
    {
        [Fact]
        public void Constructor_Id_SeAsigna()
        {
            int id = 1;
            var agente = new Agente(id);
            Assert.Equal(id, agente.Id);
        }

        [Fact]
        public void Constructor_Valoraciones_SeInicializaVacia()
        {
            var agente = new Agente(1);
            Assert.NotNull(agente.Valoraciones);
            Assert.Empty(agente.Valoraciones);
        }

        [Fact]
        public void AgregarValoracion_Valoracion_SeAgrega()
        {
            var atomo1 = new Atomo(1, 1);
            var atomo2 = new Atomo(2, 1);

            var agente = new Agente(1);
            agente.AgregarValoracion(atomo1);
            agente.AgregarValoracion(atomo2);

            Assert.True(agente.Valoraciones.Count == 2);
            Assert.Same(atomo1, agente.Valoraciones[0]);
            Assert.Same(atomo2, agente.Valoraciones[1]);
        }

        [Fact]
        public void AgregarValoracion_ValoracionSobreMismoAtomo_LanzaExcepcion()
        {
            int posicionAtomo = 1;

            var agente = new Agente(1);
            agente.AgregarValoracion(new Atomo(posicionAtomo, 1));

            var ex = Assert.Throws<InvalidOperationException>(() => agente.AgregarValoracion(new Atomo(posicionAtomo, 0)));
            Assert.Equal($"Ya existe una valoración para el átomo #{posicionAtomo}", ex.Message);
        }
    }
}
