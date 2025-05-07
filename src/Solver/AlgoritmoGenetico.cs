using Solver.Individuos;

namespace Solver
{
    internal class AlgoritmoGenetico
    {
        private readonly List<Individuo> _poblacion;

        public AlgoritmoGenetico(List<Individuo> poblacion)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));
            if (poblacion.Count == 0)
                throw new ArgumentException("La población no puede estar vacía", nameof(poblacion));

            _poblacion = poblacion;
        }

        public Individuo Ejecutar()
        {
            if (_poblacion.Count == 1)
                return _poblacion[0];

            return null;
        }
    }
}
