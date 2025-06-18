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
            int generacionActual = 0;
            bool ejecutarHastaEncontrarSolucion = _limiteGeneraciones == 0;
            bool generacionLimiteNoAlcanzada = generacionActual < _limiteGeneraciones;

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            decimal mejorFitness = mejorIndividuo.Fitness();
            int ultimaGeneracionConMejora = 0;

            while (ejecutarHastaEncontrarSolucion || generacionLimiteNoAlcanzada)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                Individuo mejorIndividuoDeGeneracion = _poblacion.ObtenerMejorIndividuo();
                decimal mejorFitnessDeGeneracion = mejorIndividuoDeGeneracion.Fitness();

                bool esSolucionOptima = mejorFitnessDeGeneracion == 0;
                if (esSolucionOptima)
                {
                    mejorIndividuo = mejorIndividuoDeGeneracion;
                    break;
                }

                bool hayEstancamiento = HayEstancamiento(ultimaGeneracionConMejora, generacionActual);
                if (hayEstancamiento)
                {
                    mejorIndividuo = mejorIndividuoDeGeneracion;
                    break;
                }

                bool hayNuevoMejorFitness = mejorFitnessDeGeneracion < mejorFitness;
                if (hayNuevoMejorFitness)
                {
                    ultimaGeneracionConMejora = generacionActual;
                    mejorFitness = mejorFitnessDeGeneracion;
                }

                generacionActual++;
                _poblacion = _poblacion.GenerarNuevaGeneracion();
                generacionLimiteNoAlcanzada = generacionActual < _limiteGeneraciones;

                GeneracionProcesada?.Invoke(generacionActual, cancellationToken);
            }

            return (mejorIndividuo, generacionActual);
        }

        private bool HayEstancamiento(int ultimaGeneracionConMejora, int generacionActual)
        {
            int cantidadGeneracionesSinMejora = generacionActual - ultimaGeneracionConMejora;
            bool limiteGeneracionesSinMejoraAlcanzado = cantidadGeneracionesSinMejora >= _limiteGeneracionesSinMejora;
            bool hayEstancamiento = _validarEstancamiento && limiteGeneracionesSinMejoraAlcanzado;
            return hayEstancamiento;
        }
    }
}
