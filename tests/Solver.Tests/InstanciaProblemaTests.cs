namespace Solver.Tests
{
    public class InstanciaProblemaTests
    {
        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizNull_LanzaArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(null));
            Assert.StartsWith("La matriz de valoraciones no puede ser null", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizVacia_LanzaArgumentException()
        {
            var matriz = new decimal[0, 0];
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz));
            Assert.StartsWith("La matriz de valoraciones no puede estar vacía", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizConAgentesSinValoraciones_LanzaArgumentException()
        {
            decimal[,] matriz = {
                { 0, 1, 1 },
                { 0, 1, 1 },
            };
            var ex = Assert.Throws<ArgumentException>(() => InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz));
            Assert.StartsWith("El agente 1 no tiene valoraciones positivas sobre ningún átomo.", ex.Message);
        }

        [Fact]
        public void CrearDesdeMatrizDeValoraciones_MatrizValida_CreaInstanciaCorrectamente()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 0, 3.9m },
                { 1, 1.2m },
                { 1, 4.6m },
                { 2, 1.5m },
                { 3, 5.3m },
                { 5, 0.0m },
            });

            Assert.Equal(2, instanciaProblema.Agentes.Count);
            Assert.Equal(6, instanciaProblema.CantidadAtomos);

            Agente agente1 = instanciaProblema.Agentes[0];
            Assert.Equal(1, agente1.Id);
            Assert.Equal(5, agente1.Valoraciones.Count);
            Assert.Contains(agente1.Valoraciones, a => a.Posicion == 2 && a.Valoracion == 1);
            Assert.Contains(agente1.Valoraciones, a => a.Posicion == 3 && a.Valoracion == 1);
            Assert.Contains(agente1.Valoraciones, a => a.Posicion == 4 && a.Valoracion == 2);
            Assert.Contains(agente1.Valoraciones, a => a.Posicion == 5 && a.Valoracion == 3);
            Assert.Contains(agente1.Valoraciones, a => a.Posicion == 6 && a.Valoracion == 5);

            Agente agente2 = instanciaProblema.Agentes[1];
            Assert.Equal(2, agente2.Id);
            Assert.Equal(5, agente2.Valoraciones.Count);
            Assert.Contains(agente2.Valoraciones, a => a.Posicion == 1 && a.Valoracion == 3.9m);
            Assert.Contains(agente2.Valoraciones, a => a.Posicion == 2 && a.Valoracion == 1.2m);
            Assert.Contains(agente2.Valoraciones, a => a.Posicion == 3 && a.Valoracion == 4.6m);
            Assert.Contains(agente2.Valoraciones, a => a.Posicion == 4 && a.Valoracion == 1.5m);
            Assert.Contains(agente2.Valoraciones, a => a.Posicion == 5 && a.Valoracion == 5.3m);
        }
    }
}
