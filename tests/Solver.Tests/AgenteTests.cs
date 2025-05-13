namespace Solver.Tests
{
    public class AgenteTests
    {
        private const int IdAgente = 1;

        private readonly Agente _agente;

        public AgenteTests()
        {
            _agente = new Agente(IdAgente);
        }

        [Fact]
        public void Constructor_Id_SeAsigna()
        {
            Assert.Equal(IdAgente, _agente.Id);
        }

        [Fact]
        public void Constructor_Valoraciones_SeInicializaVacia()
        {
            Assert.NotNull(_agente.Valoraciones);
            Assert.Empty(_agente.Valoraciones);
        }

        [Fact]
        public void AgregarValoracion_Valoracion_SeAgrega()
        {
            var atomo1 = new Atomo(1, 1);
            var atomo2 = new Atomo(2, 1);

            _agente.AgregarValoracion(atomo1);
            _agente.AgregarValoracion(atomo2);

            Assert.True(_agente.Valoraciones.Count == 2);
            Assert.Same(atomo1, _agente.Valoraciones[0]);
            Assert.Same(atomo2, _agente.Valoraciones[1]);
        }

        [Fact]
        public void AgregarValoracion_ValoracionSobreMismoAtomo_LanzaInvalidOperationException()
        {
            const int posicion = 1;

            _agente.AgregarValoracion(new Atomo(posicion, 1));

            var ex = Assert.Throws<InvalidOperationException>(() => _agente.AgregarValoracion(new Atomo(posicion, 0)));
            Assert.Contains("Ya existe valoración para el átomo", ex.Message);
        }
    }
}
