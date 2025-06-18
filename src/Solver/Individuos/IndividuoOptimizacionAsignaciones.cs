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
        int cantidadCortes = _problema.Agentes.Count - 1;
        int cantidadAsignaciones = _problema.Agentes.Count;

        if (cantidadAsignaciones <= 1)
            return;

        var cortes = Cromosoma.Take(cantidadCortes).ToList<int>();
        cortes.Sort();

        decimal[,] valoraciones = new decimal[cantidadAsignaciones, cantidadAsignaciones];
        for (int porcion = 0; porcion < cantidadAsignaciones; porcion++)
        {
            int atomoInicio = porcion == 0 ? 1 : cortes[porcion - 1] + 1;
            int atomoFin = porcion < cortes.Count ? cortes[porcion] : _problema.CantidadAtomos;

            for (int agenteIdx = 0; agenteIdx < cantidadAsignaciones; agenteIdx++)
            {
                decimal valor = 0;
                Agente agente = _problema.Agentes[agenteIdx];
                foreach (Atomo atomo in agente.Valoraciones)
                {
                    if (atomo.Posicion >= atomoInicio && atomo.Posicion <= atomoFin)
                        valor += atomo.Valoracion;
                }
                valoraciones[porcion, agenteIdx] = valor;
            }
        }

        decimal maxValor = 0;
        for (int i = 0; i < cantidadAsignaciones; i++)
            for (int j = 0; j < cantidadAsignaciones; j++)
                if (valoraciones[i, j] > maxValor)
                    maxValor = valoraciones[i, j];

        int[,] costos = new int[cantidadAsignaciones, cantidadAsignaciones];
        for (int i = 0; i < cantidadAsignaciones; i++)
        {
            for (int j = 0; j < cantidadAsignaciones; j++)
            {
                decimal costo = (maxValor - valoraciones[i, j]) * 1000m;
                costos[i, j] = (int)Math.Round(costo);
            }
        }

        int[] asignacionesOptimas = HungarianAlgorithm.HungarianAlgorithm.FindAssignments(costos);

        for (int i = 0; i < cantidadAsignaciones; i++)
        {
            Cromosoma[cantidadCortes + i] = asignacionesOptimas[i] + 1;
        }
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
