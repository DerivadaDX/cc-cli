
namespace GeneradorInstancias
{
    internal class InstanciaBuilder
    {
        private int _cantidadAtomos;
        private int _cantidadJugadores;

        internal InstanciaBuilder ConCantidadDeAtomos(int cantidadAtomos)
        {
            if (cantidadAtomos <= 0)
            {
                string mensaje = $"La cantidad de átomos debe ser mayor a cero: {cantidadAtomos}";
                throw new ArgumentOutOfRangeException(nameof(cantidadAtomos), mensaje);
            }

            _cantidadAtomos = cantidadAtomos;
            return this;
        }

        internal InstanciaBuilder ConCantidadDeJugadores(int cantidadJugadores)
        {
            if (cantidadJugadores <= 0)
            {
                string mensaje = $"La cantidad de jugadores debe ser mayor a cero: {cantidadJugadores}";
                throw new ArgumentOutOfRangeException(nameof(cantidadJugadores), mensaje);
            }

            _cantidadJugadores = cantidadJugadores;
            return this;
        }

        internal decimal[][] Build()
        {
            if (_cantidadAtomos == 0 || _cantidadJugadores == 0)
                throw new InvalidOperationException("Debe especificar el número de átomos y jugadores antes de construir la instancia");

            var instancia = new decimal[_cantidadAtomos][];

            for (int indiceAtomo = 0; indiceAtomo < _cantidadAtomos; indiceAtomo++)
            {
                instancia[indiceAtomo] = new decimal[_cantidadJugadores];
            }

            return instancia;
        }
    }
}
