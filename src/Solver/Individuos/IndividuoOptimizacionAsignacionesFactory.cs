using Common;

namespace Solver.Individuos;

public class IndividuoOptimizacionAsignacionesFactory : IIndividuoFactory
{
    private readonly InstanciaProblema _problema;
    private readonly GeneradorNumerosRandom _random;

    public IndividuoOptimizacionAsignacionesFactory(InstanciaProblema problema)
    {
        ArgumentNullException.ThrowIfNull(problema, nameof(problema));

        _problema = problema;
        _random = GeneradorNumerosRandomFactory.Crear();
    }

    public Individuo CrearAleatorio()
    {
        int cantidadAgentes = _problema.Agentes.Count;
        List<int> cortes = GenerarCortes(cantidadAgentes - 1);
        List<int> asignaciones = GenerarAsignaciones(cantidadAgentes);

        var cromosoma = cortes.Concat(asignaciones).ToList<int>();
        var individuo = new IndividuoOptimizacionAsignaciones(cromosoma, _problema);
        return individuo;
    }

    private List<int> GenerarCortes(int cantidadCortes)
    {
        var cortes = new List<int>();
        for (int i = 0; i < cantidadCortes; i++)
        {
            int corte = _random.Siguiente(_problema.CantidadAtomos + 1);
            cortes.Add(corte);
        }

        return cortes;
    }

    private List<int> GenerarAsignaciones(int cantidadAgentes)
    {
        var asignaciones = Enumerable.Range(1, cantidadAgentes).ToList();

        // Fisher–Yates shuffle
        for (int i = asignaciones.Count - 1; i > 0; i--)
        {
            int j = _random.Siguiente(i + 1);
            (asignaciones[i], asignaciones[j]) = (asignaciones[j], asignaciones[i]);
        }

        return asignaciones;
    }
}
