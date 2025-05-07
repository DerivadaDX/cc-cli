using Solver.Individuos;

namespace Solver
{
    internal class AlgoritmoGenetico
    {
        public AlgoritmoGenetico(List<Individuo> poblacion)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));
            if (poblacion.Count == 0)
                throw new ArgumentException("La población no puede estar vacía", nameof(poblacion));
        }
    }
}
