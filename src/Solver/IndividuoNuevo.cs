using System.Linq;
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
            _cromosoma = GenerarCromosomaAleatorio(tamañoCromosoma, cantidadUnos);

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
            _asignaciones = CalcularAsignacionesOptimas();
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

        private List<int> CalcularAsignacionesOptimas()
        {
            List<int> posicionesCortes = ExtraerPosicionesCortes(_cromosoma);
            decimal[,] valoracionesDePorciones =
                _calculadoraValoraciones.CalcularMatrizValoracionesPorcionAgente(_problema, posicionesCortes);

            List<int> asignaciones = _algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesDePorciones);
            return asignaciones;
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
