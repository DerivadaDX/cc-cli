namespace Solver
{
    public class IndividuoNuevo
    {
        internal IndividuoNuevo(int cantidadAtomos)
        {
            if (cantidadAtomos < 1)
            {
                string mensaje = $"La cantidad de átomos debe ser mayor o igual a 1 (valor: {cantidadAtomos})";
                throw new ArgumentOutOfRangeException(nameof(cantidadAtomos), mensaje);
            }

            Cromosoma = new List<int>(new int[cantidadAtomos - 1]);
        }

        internal List<int> Cromosoma { get; }
    }
}
