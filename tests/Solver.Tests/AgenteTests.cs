namespace Solver.Tests
{
    public class AgenteTests
    {
        [Fact]
        public void Constructor_Id_SeAsigna()
        {
            var agente = new Agente(1);
            Assert.Equal(1, agente.Id);
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

            Agente agente = ObtenerAgente();
            agente.AgregarValoracion(atomo1);
            agente.AgregarValoracion(atomo2);

            Assert.Equal(2, agente.Valoraciones.Count);
            Assert.Same(atomo1, agente.Valoraciones[0]);
            Assert.Same(atomo2, agente.Valoraciones[1]);
        }

        [Fact]
        public void AgregarValoracion_ValoracionSobreMismoAtomo_LanzaInvalidOperationException()
        {
            const int posicion = 1;

            Agente agente = ObtenerAgente();
            agente.AgregarValoracion(new Atomo(posicion, 1));

            var ex = Assert.Throws<InvalidOperationException>(() => agente.AgregarValoracion(new Atomo(posicion, 0)));
            Assert.Contains("Ya existe valoración para el átomo", ex.Message);
        }

        private Agente ObtenerAgente()
        {
            var agente = new Agente(1);
            return agente;
        }
    }
}
