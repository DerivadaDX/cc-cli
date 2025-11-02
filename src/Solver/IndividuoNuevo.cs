using Common;

namespace Solver
{
    public class IndividuoNuevo
    {
        private readonly GeneradorNumerosRandom _generadorRandom;

        internal IndividuoNuevo(int cantidadAtomos, int cantidadJugadores, GeneradorNumerosRandom generadorRandom)
        {
            if (cantidadAtomos < 1)
            {
                string mensaje = $"La cantidad de átomos debe ser mayor o igual a 1 (valor: {cantidadAtomos})";
                throw new ArgumentOutOfRangeException(nameof(cantidadAtomos), mensaje);
            }

            if (cantidadJugadores > cantidadAtomos)
            {
                string mensaje =
                    "La cantidad de jugadores no puede ser mayor que la cantidad de átomos (átomos: {0}, jugadores: {1})";
                mensaje = string.Format(mensaje, cantidadAtomos, cantidadJugadores);
                throw new ArgumentOutOfRangeException(nameof(cantidadJugadores), mensaje);
            }

            _generadorRandom = generadorRandom;

            Cromosoma = new List<int>(new int[cantidadAtomos - 1]);
            InicializarCromosomaAleatorio(cantidadJugadores - 1);
        }

        internal List<int> Cromosoma { get; }

        private void InicializarCromosomaAleatorio(int cantidadUnos)
        {
            var indicesDisponibles = Enumerable.Range(0, Cromosoma.Count).ToList<int>();
            for (int i = 0; i < cantidadUnos; i++)
            {
                int indiceRandom = _generadorRandom.Siguiente(indicesDisponibles.Count);
                int indiceSeleccionado = indicesDisponibles[indiceRandom];

                Cromosoma[indiceSeleccionado] = 1;
                indicesDisponibles.RemoveAt(indiceRandom);
            }
        }
    }
}
