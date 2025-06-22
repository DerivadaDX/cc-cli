namespace Solver.Individuos
{
    internal class IndividuoIntercambioAsignaciones : Individuo
    {
        internal IndividuoIntercambioAsignaciones(List<int> cromosoma, InstanciaProblema problema) : base(cromosoma, problema)
        {
        }

        protected override void MutarAsignaciones()
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

        protected override Individuo CrearNuevoIndividuo(List<int> cromosoma)
        {
            var individuo = new IndividuoIntercambioAsignaciones(cromosoma, _problema);
            return individuo;
        }
    }
}