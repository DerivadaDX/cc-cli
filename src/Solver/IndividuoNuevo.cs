namespace Solver
{
    public class IndividuoNuevo
    {
        internal IndividuoNuevo(int cantidadAtomos, int cantidadJugadores)
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

            Cromosoma = new List<int>(new int[cantidadAtomos - 1]);
        }

        internal List<int> Cromosoma { get; }
    }
}
