namespace Solver;

internal static class AlgoritmoHungaroFactory
{
    private static AlgoritmoHungaro _instancia = null;

    public static AlgoritmoHungaro Crear()
    {
        var instancia = _instancia ?? new AlgoritmoHungaro();
        return instancia;
    }

#if DEBUG
    /// <summary>
    /// Solo usar para tests. Solamente está disponible en modo DEBUG.
    /// </summary>
    internal static void SetearInstancia(AlgoritmoHungaro instancia)
    {
        _instancia = instancia;
    }
#endif
}
