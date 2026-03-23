using System;
using System.Collections.Generic;

namespace Solver.Tests;

public class CalculadoraValoracionesPorcionesTests
{
    [Fact]
    public void CalcularMatrizValoracionesPorcionAgente_ProblemaEsNull_LanzaArgumentNullException()
    {
        var calculadora = new CalculadoraValoracionesPorciones();

        var ex = Assert.Throws<ArgumentNullException>(
            () => calculadora.CalcularMatrizValoracionesPorcionAgente(null, []));
        Assert.Equal("problema", ex.ParamName);
    }

    [Fact]
    public void CalcularMatrizValoracionesPorcionAgente_CortesEsNull_LanzaArgumentNullException()
    {
        decimal[,] valoraciones = new decimal[,]
        {
                { 1m },
        };
        InstanciaProblema problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
        var calculadora = new CalculadoraValoracionesPorciones();

        var ex = Assert.Throws<ArgumentNullException>(
            () => calculadora.CalcularMatrizValoracionesPorcionAgente(problema, null));
        Assert.Equal("posicionesCortes", ex.ParamName);
    }

    [Fact]
    public void CalcularMatrizValoracionesPorcionAgente_CantidadCortesInsuficiente_LanzaArgumentException()
    {
        decimal[,] valoraciones = new decimal[,]
        {
            { 1m, 2m, 3m },
            { 3m, 2m, 1m },
        };
        InstanciaProblema problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(valoraciones);
        var calculadora = new CalculadoraValoracionesPorciones();
        List<int> cortes = [1];

        var ex = Assert.Throws<ArgumentException>(
            () => calculadora.CalcularMatrizValoracionesPorcionAgente(problema, cortes));
        Assert.Contains("cantidad de cortes indicada no coincide con la esperada", ex.Message);
        Assert.Equal("posicionesCortes", ex.ParamName);
    }

    [Fact]
    public void CalcularMatrizValoracionesPorcionAgente_CortesDesordenados_RetornaMatrizDeValoracionesPorcionAgente()
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
        List<int> cortes = [4, 2];

        decimal[,] resultado = calculadora.CalcularMatrizValoracionesPorcionAgente(problema, cortes);

        decimal[,] esperado = new decimal[,]
        {
            { 5m, 3m, 3m },
            { 6m, 4m, 4m },
            { 6m, 3m, 4m },
        };
        AssertMatricesIguales(esperado, resultado);
    }

    [Fact]
    public void CalcularPreferenciasPorcion_ValoracionesDePorcionesNull_LanzaArgumentNullException()
    {
        var calculadora = new CalculadoraValoracionesPorciones();

        var ex = Assert.Throws<ArgumentNullException>(() => calculadora.CalcularPreferenciasPorcion(null));
        Assert.Equal("valoracionesDePorciones", ex.ParamName);
    }

    [Fact]
    public void CalcularPreferenciasPorcion_ValoracionesDispares_RetornaConteosEsperados()
    {
        decimal[,] valoraciones =
        {
            { 5m, 1m, 4m },
            { 3m, 6m, 4m },
            { 1m, 2m, 4m },
        };
        var calculadora = new CalculadoraValoracionesPorciones();

        List<int> preferencias = calculadora.CalcularPreferenciasPorcion(valoraciones);

        Assert.Equal([2, 2, 1], preferencias);
    }

    [Fact]
    public void CalcularPreferenciasPorcion_MatrizVacia_RetornaListaVacia()
    {
        decimal[,] valoraciones = new decimal[0, 0];
        var calculadora = new CalculadoraValoracionesPorciones();

        List<int> preferencias = calculadora.CalcularPreferenciasPorcion(valoraciones);

        Assert.Empty(preferencias);
    }

    [Fact]
    public void CalcularPreferenciasPorcion_ValoracionesTodasCero_CuentaATodosLosAgentes()
    {
        decimal[,] valoraciones =
        {
            { 0m, 0m, 0m },
            { 0m, 0m, 0m },
        };
        var calculadora = new CalculadoraValoracionesPorciones();

        List<int> preferencias = calculadora.CalcularPreferenciasPorcion(valoraciones);

        Assert.Equal([3, 3], preferencias);
    }

    [Fact]
    public void CalcularPreferenciasPorcion_ValoracionesTodasIgualesNoCero_CuentaATodosLosAgentes()
    {
        decimal[,] valoraciones =
        {
            { 4m, 4m },
            { 4m, 4m },
            { 4m, 4m },
        };
        var calculadora = new CalculadoraValoracionesPorciones();

        List<int> preferencias = calculadora.CalcularPreferenciasPorcion(valoraciones);

        Assert.Equal([2, 2, 2], preferencias);
    }

    private static void AssertMatricesIguales(decimal[,] esperado, decimal[,] obtenido)
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
