namespace Solver;

internal static class CalculadoraValoracionesPorcionesFactory
{
    private static CalculadoraValoracionesPorciones _instancia = null;

    public static CalculadoraValoracionesPorciones Crear()
    {
        var instancia = _instancia ?? new CalculadoraValoracionesPorciones();
        return instancia;
    }

#if DEBUG
    /// <summary>
    /// Solo usar para tests. Solamente está disponible en modo DEBUG.
    /// </summary>
    internal static void SetearInstancia(CalculadoraValoracionesPorciones calculadora)
    {
        _instancia = calculadora;
    }
#endif
}
