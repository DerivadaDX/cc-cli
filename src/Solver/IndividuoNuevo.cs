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

        internal IndividuoNuevo(InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            _problema = problema;

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));
            _generadorRandom = generadorRandom;

            int tamañoCromosoma = problema.CantidadAtomos - 1;
            int cantidadUnos = problema.Agentes.Count - 1;
            _cromosoma = [.. new int[tamañoCromosoma]];
            InicializarCromosomaAleatorio(cantidadUnos);

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
            _asignaciones = CalcularAsignacionesOptimas();
        }

        internal IReadOnlyList<int> Cromosoma => _cromosoma;
        internal IReadOnlyList<int> Asignaciones => _asignaciones;

        private void InicializarCromosomaAleatorio(int cantidadUnos)
        {
            var indicesDisponibles = Enumerable.Range(0, Cromosoma.Count).ToList<int>();
            for (int i = 0; i < cantidadUnos; i++)
            {
                int indiceRandom = _generadorRandom.Siguiente(indicesDisponibles.Count);
                int indiceSeleccionado = indicesDisponibles[indiceRandom];

                _cromosoma[indiceSeleccionado] = 1;
                indicesDisponibles.RemoveAt(indiceRandom);
            }
        }

        private List<int> CalcularAsignacionesOptimas()
        {
            List<int> posicionesCortes = ExtraerPosicionesCortes(_cromosoma);
            decimal[,] valoracionesDePorciones = _calculadoraValoraciones.Calcular(_problema, posicionesCortes);

            int[] asignaciones = _algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesDePorciones);
            return asignaciones.ToList();
        }

        private List<int> ExtraerPosicionesCortes(List<int> cromosoma)
        {
            var posiciones = new List<int>();
            for (int indice = 0; indice < cromosoma.Count; indice++)
            {
                if (cromosoma[indice] == 1)
                    posiciones.Add(indice + 1);
            }

            return posiciones;
        }
    }
}
