public class AlgoritmoHungaroTests
{
    [Fact]
    public void CalcularAsignacionOptimaDePorciones_MatrizUnElemento_AsignaUnicaPorcionAlUnicoAgente()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 42m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas = new int[] { 0 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesSimples_RetornaAsignacionCorrecta()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 50m, 10m },
            { 10m, 50m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas = new int[] { 0, 1 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesIguales_RetornaCualquierAsignacion()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 20m, 20m },
            { 20m, 20m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas1 = new int[] { 0, 1 };
        var asignacionesEsperadas2 = new int[] { 1, 0 };

        bool esAsignacionValida =
            asignaciones.SequenceEqual(asignacionesEsperadas1) || asignaciones.SequenceEqual(asignacionesEsperadas2);
        Assert.True(esAsignacionValida, $"La asignación devuelta no es la esperada: [{string.Join(", ", asignaciones)}]");
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesEscaladasMantienenAsignacion_RetornaMismaAsignacion()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoracionesNormal =
        {
            { 8m, 4m },
            { 6m, 7m },
        };
        decimal[,] valoracionesEscaladas =
        {
            { 16m, 8m },
            { 12m, 14m },
        };

        int[] asignacionBase = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesNormal);
        int[] asignacionEscalada = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesEscaladas);

        Assert.Equal(asignacionBase, asignacionEscalada);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesConDecimales_RetornaAsignacionEsperada()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 1.234m, 0.100m },
            { 0.200m, 1.233m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas = new[] { 0, 1 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesConNegativos_RetornaAsignacionEsperada()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { -1m, -10m },
            { -20m, -2m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas = new[] { 0, 1 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_MatrizDiagonal_RetornaAsignacionIdentidad()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 9m, 1m, 2m, 3m, 4m },
            { 1m, 9m, 2m, 3m, 4m },
            { 1m, 2m, 9m, 3m, 4m },
            { 1m, 2m, 3m, 9m, 4m },
            { 1m, 2m, 3m, 4m, 9m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        var asignacionesEsperadas = new[] { 0, 1, 2, 3, 4 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesMuyCercanas_CometeErorrPorRedondeo()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 0.998348m, 0.999539m },
            { 0.998660m, 1.000000m },
        };

        int[] asignaciones = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        // Suma real:
        // Asignación [0, 1]: 0.998348 + 1.000000 = 1.998348  (óptimo real)
        // Asignación [1, 0]: 0.999539 + 0.998660 = 1.998199
        var asignacionErroneaEsperada = new[] { 1, 0 };
        Assert.Equal(asignacionErroneaEsperada, asignaciones);
    }
}
