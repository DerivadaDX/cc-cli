using Common;

namespace Solver.Individuos;

internal class IndividuoOptimizacionAsignaciones : Individuo
{
    internal IndividuoOptimizacionAsignaciones(
        List<int> cromosoma, InstanciaProblema problema, CalculadoraFitness calculadoraFitness)
        : base(cromosoma, problema, calculadoraFitness)
    {
    }

    protected override void MutarAsignaciones()
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        if (cantidadAsignaciones <= 1)
            return;

        List<int> cortes = ObtenerCortesOrdenados();
        decimal[,] matrizValoraciones = CalcularValoracionesPorPorcionYAgente(cortes);
        int[,] matrizCostos = ConvertirValoracionesACostos(matrizValoraciones);
        int[] asignacionesOptimas = HungarianAlgorithm.HungarianAlgorithm.FindAssignments(matrizCostos);

        ActualizarCromosomaConAsignaciones(asignacionesOptimas);
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

    private decimal CalcularValorPorcion(Agente agente, int atomoInicio, int atomoFin)
    {
        decimal valor = 0;
        foreach (Atomo atomo in agente.Valoraciones)
        {
            if (atomo.Posicion >= atomoInicio && atomo.Posicion <= atomoFin)
                valor += atomo.Valoracion;
        }
        return valor;
    }

    private int[,] ConvertirValoracionesACostos(decimal[,] valoraciones)
    {
        decimal valorMaximo = ObtenerValorMaximo(valoraciones);
        int porciones = valoraciones.GetLength(0);
        int agentes = valoraciones.GetLength(1);

        int[,] costos = new int[porciones, agentes];
        for (int porcion = 0; porcion < porciones; porcion++)
        {
            for (int agente = 0; agente < agentes; agente++)
            {
                // Convertir el problema de maximización a minimización
                decimal costo = (valorMaximo - valoraciones[porcion, agente]) * 1000m;
                costos[porcion, agente] = (int)Math.Round(costo);
            }
        }

        return costos;
    }

    private decimal ObtenerValorMaximo(decimal[,] matriz)
    {
        int porciones = matriz.GetLength(0);
        int agentes = matriz.GetLength(1);

        var valorMaximo = decimal.MinValue;
        for (int porcion = 0; porcion < porciones; porcion++)
        {
            for (int agente = 0; agente < agentes; agente++)
            {
                decimal valorActual = matriz[porcion, agente];
                if (valorActual > valorMaximo)
                    valorMaximo = valorActual;
            }
        }

        return valorMaximo;
    }

    private void ActualizarCromosomaConAsignaciones(int[] asignacionesOptimas)
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        int cantidadCortes = _problema.Agentes.Count - 1;

        for (int porcion = 0; porcion < cantidadAsignaciones; porcion++)
            Cromosoma[cantidadCortes + porcion] = asignacionesOptimas[porcion] + 1;
    }

    protected override Individuo CrearNuevoIndividuo(List<int> cromosoma)
    {
        return new IndividuoOptimizacionAsignaciones(cromosoma, _problema, _calculadoraFitness);
    }
}
