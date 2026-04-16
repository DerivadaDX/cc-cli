using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Solver.Tests;

public class AlgoritmoHungaroTests
{
    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesNull_LanzaArgumentNullException()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();

        var ex = Assert.Throws<ArgumentNullException>(
            () => algoritmoHungaro.CalcularAsignacionOptimaDePorciones(null));

        Assert.Equal("valoracionesDePorciones", ex.ParamName);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_MatrizNoCuadrada_LanzaArgumentException()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 5m, 4m, 3m },
            { 2m, 1m, 0m },
        };

        var ex = Assert.Throws<ArgumentException>(
            () => algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones));

        Assert.Equal("valoracionesDePorciones", ex.ParamName);
        Assert.Contains("debe ser cuadrada", ex.Message);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_MatrizUnElemento_AsignaUnicaPorcionAlUnicoAgente()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 42m },
        };

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada = [0];
        Assert.Equal(asignacionEsperada, asignacion);
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

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada = [0, 1];
        Assert.Equal(asignacionEsperada, asignacion);
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

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada1 = [0, 1];
        List<int> asignacionEsperada2 = [1, 0];

        bool esAsignacionValida = asignacion.SequenceEqual(asignacionEsperada1) || asignacion.SequenceEqual(asignacionEsperada2);
        Assert.True(esAsignacionValida, $"La asignación devuelta no es la esperada: [{string.Join(", ", asignacion)}]");
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

        ImmutableArray<int> asignacionValoracionesBase = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesNormal);
        ImmutableArray<int> asignacionValoracionesEscaladas = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesEscaladas);

        Assert.Equal(asignacionValoracionesBase, asignacionValoracionesEscaladas);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesConDecimales_Retornaasignacionperada()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 1.234m, 0.100m },
            { 0.200m, 1.233m },
        };

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada = [0, 1];
        Assert.Equal(asignacionEsperada, asignacion);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesConNegativos_Retornaasignacionperada()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { -1m, -10m },
            { -20m, -2m },
        };

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada = [0, 1];
        Assert.Equal(asignacionEsperada, asignacion);
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

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        List<int> asignacionEsperada = [0, 1, 2, 3, 4];
        Assert.Equal(asignacionEsperada, asignacion);
    }

    [Fact]
    public void CalcularAsignacionOptimaDePorciones_ValoracionesMuyCercanas_NoPierdePrecisionEnLaAsignacion()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones =
        {
            { 0.998348m, 0.999539m },
            { 0.998660m, 1.000000m },
        };

        ImmutableArray<int> asignacion = algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);

        // Suma real:
        // Asignación [0, 1]: 0.998348 + 1.000000 = 1.998348  (óptimo real)
        // Asignación [1, 0]: 0.999539 + 0.998660 = 1.998199
        List<int> asignacionOptimaEsperada = [0, 1];
        Assert.Equal(asignacionOptimaEsperada, asignacion);
    }
}
