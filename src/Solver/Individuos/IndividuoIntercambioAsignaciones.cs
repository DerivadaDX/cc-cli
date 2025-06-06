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
        var otroIndividuo = (IndividuoIntercambioAsignaciones)otro;

        int cantidadCortes = _problema.Agentes.Count - 1;
        int cantidadAsignaciones = _problema.Agentes.Count;

        var cortesPadre1 = _cromosoma.Take(cantidadCortes).ToList<int>();
        var cortesPadre2 = otroIndividuo._cromosoma.Take(cantidadCortes).ToList<int>();

        List<int> cortesHijo = [];
        if (cantidadCortes > 0)
        {
            int p = cantidadCortes > 1 ? _random.Siguiente(1, cantidadCortes) : cantidadCortes;
            cortesHijo.AddRange(cortesPadre1.Take(p));
            cortesHijo.AddRange(cortesPadre2.Skip(p));
        }

        var asignacionesPadre1 = _cromosoma.Skip(cantidadCortes).ToList<int>();
        var asignacionesPadre2 = otroIndividuo._cromosoma.Skip(cantidadCortes).ToList<int>();
        List<int> asignacionesHijo = [];

        if (cantidadAsignaciones > 0)
        {
            if (cantidadAsignaciones > 1)
            {
                int a = _random.Siguiente(cantidadAsignaciones);
                int b = _random.Siguiente(a + 1, cantidadAsignaciones);
                asignacionesHijo = Enumerable.Repeat(0, cantidadAsignaciones).ToList();

                var usados = new HashSet<int>();
                for (int i = a; i <= b; i++)
                {
                    asignacionesHijo[i] = asignacionesPadre1[i];
                    usados.Add(asignacionesPadre1[i]);
                }

                int indiceDestino = (b + 1) % cantidadAsignaciones;
                int indiceFuente = (b + 1) % cantidadAsignaciones;

                while (usados.Count < cantidadAsignaciones)
                {
                    int elemento = asignacionesPadre2[indiceFuente];
                    if (!usados.Contains(elemento))
                    {
                        asignacionesHijo[indiceDestino] = elemento;
                        usados.Add(elemento);
                        indiceDestino = (indiceDestino + 1) % cantidadAsignaciones;
                    }

                    indiceFuente = (indiceFuente + 1) % cantidadAsignaciones;
                }
            }
            else
            {
                asignacionesHijo.AddRange(asignacionesPadre1);
            }
        }

        List<int> cromosomaHijo = [];
        cromosomaHijo.AddRange(cortesHijo);
        cromosomaHijo.AddRange(asignacionesHijo);

        var hijo = new IndividuoIntercambioAsignaciones(cromosomaHijo, _problema, _calculadoraFitness);
        return hijo;
    }

    private void MutarCortes()
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        int L = _cromosoma.Count;

        for (int i = 0; i < cantidadCortes; i++)
        {
            if (_random.SiguienteDouble() < 1.0 / L)
            {
                int direccion = _random.Siguiente(2) == 0 ? -1 : 1;
                int nuevoValor = _cromosoma[i] + direccion;
                nuevoValor = Math.Clamp(nuevoValor, 0, _problema.CantidadAtomos);
                _cromosoma[i] = nuevoValor;
            }
        }
    }

    private void MutarAsignaciones()
    {
        int cantidadCortes = _problema.Agentes.Count - 1;
        int cantidadAsignaciones = _problema.Agentes.Count;
        int L = _cromosoma.Count;

        if (cantidadAsignaciones <= 1)
            return;

        for (int idxActual = cantidadCortes; idxActual < L; idxActual++)
        {
            if (_random.SiguienteDouble() < 1.0 / L)
            {
                int idxDestino = _random.Siguiente(cantidadAsignaciones) + cantidadCortes;
                (_cromosoma[idxDestino], _cromosoma[idxActual]) = (_cromosoma[idxActual], _cromosoma[idxDestino]);
            }
        }
    }
}

