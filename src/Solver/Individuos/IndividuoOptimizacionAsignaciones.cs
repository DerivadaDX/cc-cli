using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common;

namespace Solver.Individuos;

internal class IndividuoOptimizacionAsignaciones : IndividuoLegacy
{
    private readonly AlgoritmoHungaro _algoritmoHungaro;

    internal IndividuoOptimizacionAsignaciones(
        List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        : base(cromosoma, problema, generadorRandom)
    {
        _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
    }

    protected override void MutarAsignaciones()
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        if (cantidadAsignaciones <= 1)
            return;

        List<int> cortes = ObtenerCortesOrdenados();
        decimal[,] matrizValoraciones = CalcularValoracionesPorPorcionYAgente(cortes);
        ImmutableArray<int> asignacionesOptimas = _algoritmoHungaro.CalcularAsignacionOptimaDePorciones(matrizValoraciones);

        ActualizarCromosomaConAsignaciones(asignacionesOptimas);
    }

    protected override Individuo CrearNuevoIndividuo(List<int> cromosoma)
    {
        var individuo = new IndividuoOptimizacionAsignaciones(cromosoma, _problema, _generadorRandom);
        return individuo;
    }

    private static decimal CalcularValorPorcion(Agente agente, int atomoInicio, int atomoFin)
    {
        decimal valor = 0;
        foreach (Atomo atomo in agente.Valoraciones)
        {
            if (atomo.Posicion >= atomoInicio && atomo.Posicion <= atomoFin)
                valor += atomo.Valoracion;
        }

        return valor;
    }

    private List<int> ObtenerCortesOrdenados()
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        var cortes = Cromosoma.Take(cantidadCortes).OrderBy(x => x).ToList<int>();
        return cortes;
    }

    private decimal[,] CalcularValoracionesPorPorcionYAgente(List<int> cortes)
    {
        int cantidadAgentes = _problema.Agentes.Count;
        decimal[,] valoraciones = new decimal[cantidadAgentes, cantidadAgentes];

        for (int porcion = 0; porcion < cantidadAgentes; porcion++)
        {
            int atomoInicio = porcion > 0 ? cortes[porcion - 1] + 1 : 1;
            int atomoFin = porcion < cortes.Count ? cortes[porcion] : _problema.CantidadAtomos;

            for (int agenteIdx = 0; agenteIdx < cantidadAgentes; agenteIdx++)
            {
                Agente agente = _problema.Agentes[agenteIdx];
                decimal valorPorcion = CalcularValorPorcion(agente, atomoInicio, atomoFin);
                valoraciones[porcion, agenteIdx] = valorPorcion;
            }
        }

        return valoraciones;
    }

    private void ActualizarCromosomaConAsignaciones(ImmutableArray<int> asignacionesOptimas)
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        int cantidadCortes = _problema.Agentes.Count - 1;

        for (int porcion = 0; porcion < cantidadAsignaciones; porcion++)
            ActualizarGen(cantidadCortes + porcion, asignacionesOptimas[porcion] + 1);
    }
}
