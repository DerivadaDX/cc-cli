using Solver.Individuos;

namespace Solver
{
    internal class AlgoritmoGenetico
    {
        private readonly Poblacion _poblacion;

        public AlgoritmoGenetico(Poblacion poblacion)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));
            _poblacion = poblacion;
        }

        public Individuo Ejecutar()
        {
            if (_poblacion.Individuos.Count == 1)
                return _poblacion.Individuos[0];

            return null;
        }
    }
}
