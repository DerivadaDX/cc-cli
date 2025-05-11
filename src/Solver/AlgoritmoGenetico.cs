using Solver.Individuos;

namespace Solver
{
    internal class AlgoritmoGenetico
    {
        private Poblacion _poblacion;
        private readonly int _maxGeneraciones;
        private readonly Func<Individuo, bool> _esSolucionOptima;

        internal AlgoritmoGenetico(Poblacion poblacion, int maxGeneraciones, Func<Individuo, bool> esSolucionOptima)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));
            ArgumentNullException.ThrowIfNull(esSolucionOptima, nameof(esSolucionOptima));

            if (maxGeneraciones <= 0)
            {
                string mensaje = "El número máximo de generaciones debe ser mayor a cero.";
                throw new ArgumentOutOfRangeException(nameof(maxGeneraciones), mensaje);
            }

            _poblacion = poblacion;
            _maxGeneraciones = maxGeneraciones;
            _esSolucionOptima = esSolucionOptima;
        }

        internal Individuo Ejecutar()
        {
            for (int generacion = 0; generacion < _maxGeneraciones; generacion++)
            {
                foreach (Individuo individuo in _poblacion.Individuos)
                {
                    if (_esSolucionOptima(individuo))
                        return individuo;
                }

                _poblacion = _poblacion.GenerarNuevaGeneracion();
            }

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            return mejorIndividuo;
        }
    }
}
