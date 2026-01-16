namespace Solver.Tests
{
    public class CalculadoraValoracionesPorcionesTests
    {
        [Fact]
        public void Calcular_ProblemaEsNull_LanzaArgumentNullException()
        {
            var calculadora = new CalculadoraValoracionesPorciones();

            var ex = Assert.Throws<ArgumentNullException>(() => calculadora.Calcular(null, []));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Calcular_CortesEsNull_LanzaArgumentNullException()
        {
            decimal[,] valoraciones = new decimal[,]
            {
                { 1m },
            };
            InstanciaProblema problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
            var calculadora = new CalculadoraValoracionesPorciones();

            var ex = Assert.Throws<ArgumentNullException>(() => calculadora.Calcular(problema, null));
            Assert.Equal("posicionesCortes", ex.ParamName);
        }

        [Fact]
        public void Calcular_CantidadCortesInsuficiente_LanzaArgumentException()
        {
            decimal[,] valoraciones = new decimal[,]
            {
                { 1m, 2m, 3m },
                { 3m, 2m, 1m },
            };
            InstanciaProblema problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
            var calculadora = new CalculadoraValoracionesPorciones();
            var cortes = new List<int> { 1 };

            var ex = Assert.Throws<ArgumentException>(() => calculadora.Calcular(problema, cortes));
            Assert.Contains("cantidad de cortes indicada no coincide con la esperada", ex.Message);
            Assert.Equal("posicionesCortes", ex.ParamName);
        }

        [Fact]
        public void Calcular_CortesDesordenados_RetornaMatrizDeValoracionesPorcionAgente()
        {
            decimal[,] valoraciones = new decimal[,]
            {
                { 3m, 1m, 2m },
                { 2m, 2m, 1m },
                { 5m, 1m, 2m },
                { 1m, 3m, 2m },
                { 4m, 1m, 3m },
                { 2m, 2m, 1m },
            };
            InstanciaProblema problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
            var calculadora = new CalculadoraValoracionesPorciones();
            var cortes = new List<int> { 4, 2 };

            decimal[,] resultado = calculadora.Calcular(problema, cortes);

            decimal[,] esperado = new decimal[,]
            {
                { 5m, 3m, 3m },
                { 6m, 4m, 4m },
                { 6m, 3m, 4m },
            };
            AssertMatricesIguales(esperado, resultado);
        }

        private void AssertMatricesIguales(decimal[,] esperado, decimal[,] obtenido)
        {
            Assert.Equal(esperado.GetLength(0), obtenido.GetLength(0));
            Assert.Equal(esperado.GetLength(1), obtenido.GetLength(1));

            for (int fila = 0; fila < esperado.GetLength(0); fila++)
            {
                for (int columna = 0; columna < esperado.GetLength(1); columna++)
                {
                    Assert.Equal(esperado[fila, columna], obtenido[fila, columna]);
                }
            }
        }
    }
}
