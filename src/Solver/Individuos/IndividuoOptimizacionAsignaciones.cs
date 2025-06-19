using Common;

namespace Solver.Individuos;

internal class IndividuoOptimizacionAsignaciones : Individuo
{
    private readonly GeneradorNumerosRandom _random;

    internal IndividuoOptimizacionAsignaciones(
        List<int> cromosoma, InstanciaProblema problema, CalculadoraFitness calculadoraFitness)
        : base(cromosoma, problema, calculadoraFitness)
    {
        _random = GeneradorNumerosRandomFactory.Crear();
    }

    internal override void Mutar()
    {
        MutarCortes();
        OptimizarAsignaciones();
    }

    internal override Individuo Cruzar(Individuo otro)
    {
        if (Cromosoma.Count != otro.Cromosoma.Count)
            throw new ArgumentException("Los padres deben tener la misma cantidad de cromosomas para poder cruzarlos.");

        List<int> cortesHijo = CruzaCortes(otro);
        List<int> asignacionesHijo = CruzaAsignaciones(otro);

        List<int> cromosomaHijo = [];
        cromosomaHijo.AddRange(cortesHijo);
        cromosomaHijo.AddRange(asignacionesHijo);

        var hijo = new IndividuoOptimizacionAsignaciones(cromosomaHijo, _problema, _calculadoraFitness);
        return hijo;
    }

    private void MutarCortes()
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        int rango = _problema.CantidadAtomos + 1;
        int L = Cromosoma.Count;

        for (int i = 0; i < cantidadCortes; i++)
        {
            double probabilidadMutacion = _random.SiguienteDouble();
            if (probabilidadMutacion < 1.0 / L)
            {
                int direccion = _random.Siguiente(2) == 0 ? -1 : 1;
                int nuevoValor = (Cromosoma[i] + direccion + rango) % rango;
                Cromosoma[i] = nuevoValor;
            }
        }
    }

    private void OptimizarAsignaciones()
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        if (cantidadAsignaciones <= 1)
            return;

        List<int> cortes = ObtenerCortesOrdenados();
        decimal[,] matrizValoraciones = CalcularValoracionesPorPorcionYAgente(cortes, cantidadAsignaciones);
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

    private decimal[,] CalcularValoracionesPorPorcionYAgente(List<int> cortes, int cantidadAgentes)
    {
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
        int dimension = valoraciones.GetLength(0);
        decimal valorMaximo = ObtenerValorMaximo(valoraciones);

        int[,] costos = new int[dimension, dimension];
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                // Convertir el problema de maximización a minimización
                // multiplicamos por 1000 para mantener la precisión al convertir a entero
                decimal costo = (valorMaximo - valoraciones[i, j]) * 1000m;
                costos[i, j] = (int)Math.Round(costo);
            }
        }

        return costos;
    }

    private decimal ObtenerValorMaximo(decimal[,] matriz)
    {
        int dimension = matriz.GetLength(0);
        decimal valorMaximo = decimal.MinValue;

        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                if (matriz[i, j] > valorMaximo)
                    valorMaximo = matriz[i, j];
            }
        }

        return valorMaximo;
    }

    private void ActualizarCromosomaConAsignaciones(int[] asignacionesOptimas)
    {
        int cantidadAsignaciones = _problema.Agentes.Count;
        int cantidadCortes = _problema.Agentes.Count - 1;

        for (int i = 0; i < cantidadAsignaciones; i++)
            Cromosoma[cantidadCortes + i] = asignacionesOptimas[i] + 1;
    }

    private List<int> CruzaCortes(Individuo otro)
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        var cortesPadre1 = Cromosoma.Take(cantidadCortes).ToList<int>();
        var cortesPadre2 = otro.Cromosoma.Take(cantidadCortes).ToList<int>();

        if (cantidadCortes == 0)
            return [];

        int indiceCorte = cantidadCortes > 1 ? _random.Siguiente(1, cantidadCortes) : cantidadCortes;
        var cortesHijo = cortesPadre1.Take(indiceCorte).ToList<int>();
        cortesHijo.AddRange(cortesPadre2.Skip(indiceCorte));
        return cortesHijo;
    }

    private List<int> CruzaAsignaciones(Individuo otro)
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        int cantidadAsignaciones = _problema.Agentes.Count;

        var asignacionesPadre1 = Cromosoma.Skip(cantidadCortes).ToList<int>();
        var asignacionesPadre2 = otro.Cromosoma.Skip(cantidadCortes).ToList<int>();

        if (cantidadAsignaciones <= 1)
            return [.. asignacionesPadre1];

        var asignacionesHijo = Enumerable.Repeat(-1, cantidadAsignaciones).ToList();
        int indiceInicioSegmento = _random.Siguiente(cantidadAsignaciones);
        int indiceFinSegmento = _random.Siguiente(indiceInicioSegmento, cantidadAsignaciones);

        for (int i = indiceInicioSegmento; i <= indiceFinSegmento; i++)
            asignacionesHijo[i] = asignacionesPadre1[i];

        int indicePadre2 = 0;
        int indiceHijo = (indiceFinSegmento + 1) % cantidadAsignaciones;
        while (asignacionesHijo.Contains(-1))
        {
            int candidato = asignacionesPadre2[indicePadre2++];
            if (!asignacionesHijo.Contains(candidato))
            {
                asignacionesHijo[indiceHijo] = candidato;
                indiceHijo = (indiceHijo + 1) % cantidadAsignaciones;
            }
        }

        return asignacionesHijo;
    }
}
