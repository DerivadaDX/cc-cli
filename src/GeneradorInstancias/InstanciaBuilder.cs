
namespace GeneradorInstancias
{
    internal class InstanciaBuilder
    {
        private int _cantidadAtomos;
        private int _cantidadJugadores;

        internal InstanciaBuilder ConAtomos(int cantidadAtomos)
        {
            _cantidadAtomos = cantidadAtomos;
            return this;
        }

        internal InstanciaBuilder ConJugadores(int cantidadJugadores)
        {
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
