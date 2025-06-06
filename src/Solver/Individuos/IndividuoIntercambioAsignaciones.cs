using Common;

namespace Solver.Individuos;

internal class IndividuoIntercambioAsignaciones : Individuo
{
    private readonly GeneradorNumerosRandom _random;

    internal IndividuoIntercambioAsignaciones(
        List<int> cromosoma, InstanciaProblema problema, CalculadoraFitness calculadoraFitness)
        : base(cromosoma, problema, calculadoraFitness)
    {
        _random = GeneradorNumerosRandomFactory.Crear();
    }

    internal override void Mutar()
    {
        MutarCortes();
        MutarAsignaciones();
    }

    internal override Individuo Cruzar(Individuo otro)
    {
        List<int> cortesHijo = CruzaCortes(otro);
        List<int> asignacionesHijo = CruzaAsignaciones(otro);

        List<int> cromosomaHijo = [];
        cromosomaHijo.AddRange(cortesHijo);
        cromosomaHijo.AddRange(asignacionesHijo);

        var hijo = new IndividuoIntercambioAsignaciones(cromosomaHijo, _problema, _calculadoraFitness);
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

    private void MutarAsignaciones()
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        int cantidadAsignaciones = _problema.Agentes.Count;
        int L = Cromosoma.Count;

        if (cantidadAsignaciones <= 1)
            return;

        for (int idxActual = cantidadCortes; idxActual < L; idxActual++)
        {
            double probabilidadMutacion = _random.SiguienteDouble();
            if (probabilidadMutacion < 1.0 / L)
            {
                int idxDestino = _random.Siguiente(cantidadCortes, L);
                (Cromosoma[idxDestino], Cromosoma[idxActual]) = (Cromosoma[idxActual], Cromosoma[idxDestino]);
            }
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
        int indiceInicioSegmento = _random.Siguiente(cantidadAsignaciones - 1);
        int indiceFinSegmento = _random.Siguiente(indiceInicioSegmento + 1, cantidadAsignaciones);

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

