using System;

namespace Solver.Tests;

public class CalculadoraValoracionesPorcionesFactoryTests : IDisposable
{
    public void Dispose()
    {
        CalculadoraValoracionesPorcionesFactory.SetearInstancia(null);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Crear_InstanciaDevuelta_EsValida()
    {
        var calculadora = CalculadoraValoracionesPorcionesFactory.Crear();

        Assert.NotNull(calculadora);
        Assert.IsType<CalculadoraValoracionesPorciones>(calculadora);
    }

    [Fact]
    public void SetearInstancia_InstanciaSeteada_EsDevueltaPorElFactory()
    {
        var calculadoraSeteada = new CalculadoraValoracionesPorciones();
        CalculadoraValoracionesPorcionesFactory.SetearInstancia(calculadoraSeteada);

        var calculadoraObtenida = CalculadoraValoracionesPorcionesFactory.Crear();

        Assert.Same(calculadoraSeteada, calculadoraObtenida);
    }
}
