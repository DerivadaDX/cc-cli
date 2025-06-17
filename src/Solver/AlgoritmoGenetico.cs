using Solver.Individuos;

namespace Solver
{
    public delegate void GeneracionProcesadaEventHandler(int generacion, CancellationToken cancellationToken);

    public class AlgoritmoGenetico
    {
        private Poblacion _poblacion;
        private readonly int _limiteGeneraciones;
        private readonly int _limiteGeneracionesSinMejora;
        private readonly bool _validarEstancamiento;

        public event GeneracionProcesadaEventHandler GeneracionProcesada;

        public AlgoritmoGenetico(Poblacion poblacion, int limiteGeneraciones, int limiteGeneracionesSinMejora)
        {
            ArgumentNullException.ThrowIfNull(poblacion, nameof(poblacion));

            if (limiteGeneraciones < 0)
            {
                string mensaje = "El límite de generaciones no puede ser negativo.";
                throw new ArgumentOutOfRangeException(nameof(limiteGeneraciones), mensaje);
            }

            if (limiteGeneracionesSinMejora < 0)
            {
                string mensaje = "El límite de generaciones sin mejora no puede ser negativo.";
                throw new ArgumentOutOfRangeException(nameof(limiteGeneracionesSinMejora), mensaje);
            }

            _poblacion = poblacion;
            _limiteGeneraciones = limiteGeneraciones;
            _limiteGeneracionesSinMejora = limiteGeneracionesSinMejora;
            _validarEstancamiento = limiteGeneracionesSinMejora > 0;
        }

        public (Individuo mejorIndividuo, int generaciones) Ejecutar(CancellationToken cancellationToken = default)
        {
            int cantidadGeneracionesProcesadas = 0;
            bool ejecutarHastaEncontrarSolucion = _limiteGeneraciones == 0;
            bool generacionLimiteNoAlcanzada = cantidadGeneracionesProcesadas < _limiteGeneraciones;

            int generacionesSinMejora = 0;
            decimal mejorFitness = decimal.MaxValue;

            while (ejecutarHastaEncontrarSolucion || generacionLimiteNoAlcanzada)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                decimal mejorFitnessDeGeneracion = decimal.MaxValue;

                foreach (Individuo individuo in _poblacion.Individuos)
                {
                    decimal fitnessIndividuoActual = individuo.Fitness();

                    bool esSolucionOptima = fitnessIndividuoActual == 0;
                    if (esSolucionOptima)
                        return (individuo, cantidadGeneracionesProcesadas);

                    bool esMejorFitnessDeGeneracion = fitnessIndividuoActual < mejorFitnessDeGeneracion;
                    if (esMejorFitnessDeGeneracion)
                        mejorFitnessDeGeneracion = fitnessIndividuoActual;
                }

                bool esGeneracionSinMejora = mejorFitnessDeGeneracion >= mejorFitness;
                if (esGeneracionSinMejora)
                {
                    generacionesSinMejora++;

                    bool limiteGeneracionesSinMejoraAlcanzado = generacionesSinMejora >= _limiteGeneracionesSinMejora;
                    if (_validarEstancamiento && limiteGeneracionesSinMejoraAlcanzado)
                        break;
                }
                else
                {
                    generacionesSinMejora = 0;
                    mejorFitness = mejorFitnessDeGeneracion;
                }

                cantidadGeneracionesProcesadas++;
                _poblacion = _poblacion.GenerarNuevaGeneracion();
                generacionLimiteNoAlcanzada = cantidadGeneracionesProcesadas < _limiteGeneraciones;

                GeneracionProcesada?.Invoke(cantidadGeneracionesProcesadas, cancellationToken);
            }

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            return (mejorIndividuo, cantidadGeneracionesProcesadas);
        }
    }
}
