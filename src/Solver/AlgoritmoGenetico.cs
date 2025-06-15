using Solver.Individuos;

namespace Solver
{
    public delegate void GeneracionProcesadaEventHandler(int generacion);

    public class AlgoritmoGenetico
    {
        private Poblacion _poblacion;
        private readonly int _limiteGeneraciones;

        public event GeneracionProcesadaEventHandler GeneracionProcesada;

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
            int cantidadGeneracionesProcesadas = 0;
            bool ejecutarHastaEncontrarSolucion = _limiteGeneraciones == 0;
            bool generacionLimiteNoAlcanzada = cantidadGeneracionesProcesadas < _limiteGeneraciones;

            while (ejecutarHastaEncontrarSolucion || generacionLimiteNoAlcanzada)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                foreach (Individuo individuo in _poblacion.Individuos)
                {
                    bool esSolucionOptima = individuo.Fitness() == 0;
                    if (esSolucionOptima)
                        return (individuo, cantidadGeneracionesProcesadas);
                }

                cantidadGeneracionesProcesadas++;
                _poblacion = _poblacion.GenerarNuevaGeneracion();
                generacionLimiteNoAlcanzada = cantidadGeneracionesProcesadas < _limiteGeneraciones;

                GeneracionProcesada?.Invoke(cantidadGeneracionesProcesadas);
            }

            Individuo mejorIndividuo = _poblacion.ObtenerMejorIndividuo();
            return (mejorIndividuo, cantidadGeneracionesProcesadas);
        }
    }
}
