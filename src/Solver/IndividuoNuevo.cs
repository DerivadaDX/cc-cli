using Common;
using System.Linq;

namespace Solver
{
    public class IndividuoNuevo
    {
        private readonly GeneradorNumerosRandom _generadorRandom;
        private readonly InstanciaProblema _problema;
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

            var algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
            var calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            decimal[,] valoracionesDePorciones = calculadoraValoraciones.Calcular(_problema, Cromosoma);
            _asignaciones = [.. algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoracionesDePorciones)];
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
    }
}
