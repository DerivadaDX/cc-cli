using Common;

namespace Solver
{
    public class IndividuoNuevo
    {
        private readonly InstanciaProblema _problema;
        private readonly GeneradorNumerosRandom _generadorRandom;
        private readonly CalculadoraValoracionesPorciones _calculadoraValoraciones;
        private readonly AlgoritmoHungaro _algoritmoHungaro;
        private readonly List<int> _cromosoma;
        private readonly List<int> _asignaciones;
        private readonly List<int> _preferenciasPorcion;

        internal IndividuoNuevo(InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            _problema = problema;

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));
            _generadorRandom = generadorRandom;

            int tamañoCromosoma = problema.CantidadAtomos - 1;
            int cantidadUnos = problema.Agentes.Count - 1;
            _cromosoma = GenerarCromosomaAleatorio(tamañoCromosoma, cantidadUnos);

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();

            decimal[,] valoracionesDePorciones = CalcularMatrizValoracionesPorcionAgente();
            _asignaciones = _algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesDePorciones);
            _preferenciasPorcion = _calculadoraValoraciones.CalcularPreferenciasPorcion(valoracionesDePorciones);
        }

        internal IReadOnlyList<int> Cromosoma => _cromosoma;
        internal IReadOnlyList<int> Asignaciones => _asignaciones;

        private List<int> GenerarCromosomaAleatorio(int tamaño, int cantidadUnos)
        {
            var cromosoma = Enumerable.Repeat(0, tamaño).ToList<int>();
            var indicesDisponibles = Enumerable.Range(0, tamaño).ToList<int>();
            for (int i = 0; i < cantidadUnos; i++)
            {
                int indiceRandom = _generadorRandom.Siguiente(indicesDisponibles.Count);
                int indiceSeleccionado = indicesDisponibles[indiceRandom];

                cromosoma[indiceSeleccionado] = 1;
                indicesDisponibles.RemoveAt(indiceRandom);
            }

            return cromosoma;
        }

        private decimal[,] CalcularMatrizValoracionesPorcionAgente()
        {
            var posicionesCortes = new List<int>();
            for (int indice = 0; indice < _cromosoma.Count; indice++)
            {
                if (_cromosoma[indice] == 1)
                    posicionesCortes.Add(indice + 1);
            }

            decimal[,] valoraciones =
                _calculadoraValoraciones.CalcularMatrizValoracionesPorcionAgente(_problema, posicionesCortes);
            return valoraciones;
        }
    }
}
