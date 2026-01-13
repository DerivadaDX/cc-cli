using Common;

namespace Solver
{
    public class IndividuoNuevo
    {
        private readonly GeneradorNumerosRandom _generadorRandom;
        private readonly List<int> _cromosoma;

        internal IndividuoNuevo(int cantidadAtomos, int cantidadAgentes, GeneradorNumerosRandom generadorRandom)
        {
            if (cantidadAtomos < 1)
            {
                string mensaje = $"La cantidad de átomos debe ser mayor o igual a 1 (valor: {cantidadAtomos})";
                throw new ArgumentOutOfRangeException(nameof(cantidadAtomos), mensaje);
            }

            if (cantidadAgentes > cantidadAtomos)
            {
                string mensaje = "La cantidad de agentes no puede ser mayor que la cantidad de átomos (átomos: {0}, agentes: {1})";
                mensaje = string.Format(mensaje, cantidadAtomos, cantidadAgentes);
                throw new ArgumentOutOfRangeException(nameof(cantidadAgentes), mensaje);
            }

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));
            _generadorRandom = generadorRandom;

            _cromosoma = [.. new int[cantidadAtomos - 1]];
            InicializarCromosomaAleatorio(cantidadAgentes - 1);
        }

        internal IReadOnlyList<int> Cromosoma => _cromosoma;

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
