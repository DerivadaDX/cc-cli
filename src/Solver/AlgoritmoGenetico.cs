using Solver.Individuos;

namespace Solver
{
    internal class AlgoritmoGenetico
    {
        public AlgoritmoGenetico(List<Individuo> poblacion)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));
        }
    }
}
