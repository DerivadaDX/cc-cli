using Solver.Individuos;
using System.Threading;

namespace Solver
{
    public class AlgoritmoGenetico
    {
        private Poblacion _poblacion;
        private readonly int _limiteGeneraciones;

        public AlgoritmoGenetico(Poblacion poblacion, int limiteGeneraciones)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));

            if (limiteGeneraciones < 0)
            {
                string mensaje = "El límite de generaciones no puede ser negativo.";
                throw new ArgumentOutOfRangeException(nameof(limiteGeneraciones), mensaje);
            }

            _poblacion = poblacion;
            _limiteGeneraciones = limiteGeneraciones;
        }

        public (Individuo mejorIndividuo, int generaciones) Ejecutar(CancellationToken cancellationToken = default)
        {
            int generacion = 0;
            bool ejecutarHastaEncontrarSolucion = _limiteGeneraciones == 0;
            bool generacionLimiteNoAlcanzada = generacion < _limiteGeneraciones;

            while (ejecutarHastaEncontrarSolucion || generacionLimiteNoAlcanzada)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                foreach (Individuo individuo in _poblacion.Individuos)
                {
                    bool esSolucionOptima = individuo.Fitness() == 0;
                    if (esSolucionOptima)
                        return (individuo, generacion);
                }

                _poblacion = _poblacion.GenerarNuevaGeneracion();
                generacionLimiteNoAlcanzada = ++generacion < _limiteGeneraciones;
            }

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            return (mejorIndividuo, generacion);
        }
    }
}
