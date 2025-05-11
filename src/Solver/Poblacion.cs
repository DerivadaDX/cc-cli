using Solver.Individuos;

namespace Solver
{
    internal class Poblacion
    {
        internal virtual List<Individuo> Individuos { get; private set; }

        internal virtual Poblacion GenerarNuevaGeneracion()
        {
            throw new NotImplementedException();
        }

        internal virtual Individuo ObtenerMejorIndividuo()
        {
            throw new NotImplementedException();
        }
    }
}
