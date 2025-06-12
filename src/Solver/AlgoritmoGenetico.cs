using Solver.Individuos;

namespace Solver
{
    public class AlgoritmoGenetico
    {
        private Poblacion _poblacion;
        private readonly int _maxGeneraciones;

        public AlgoritmoGenetico(Poblacion poblacion, int maxGeneraciones)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));

            if (maxGeneraciones < 0)
            {
                string mensaje = "El número máximo de generaciones no puede ser negativo.";
                throw new ArgumentOutOfRangeException(nameof(maxGeneraciones), mensaje);
            }

            _poblacion = poblacion;
            _maxGeneraciones = maxGeneraciones;
        }

        public (Individuo mejorIndividuo, int generaciones) Ejecutar()
        {
            int generacion = 0;
            bool ejecutarHastaEncontrarSolucion = _maxGeneraciones == 0;
            bool generacionLimiteNoAlcanzada = generacion < _maxGeneraciones;

            while (ejecutarHastaEncontrarSolucion || generacionLimiteNoAlcanzada)
            {
                foreach (Individuo individuo in _poblacion.Individuos)
                {
                    bool esSolucionOptima = individuo.Fitness() == 0;
                    if (esSolucionOptima)
                        return (individuo, generacion);
                }

                _poblacion = _poblacion.GenerarNuevaGeneracion();
                generacionLimiteNoAlcanzada = ++generacion < _maxGeneraciones;
            }

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            return (mejorIndividuo, generacion);
        }
    }
}
