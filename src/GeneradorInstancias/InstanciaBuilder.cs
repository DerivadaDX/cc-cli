
using Common;

namespace GeneradorInstancias
{
    internal class InstanciaBuilder
    {
        private int _cantidadAtomos;
        private int _cantidadJugadores;
        private int _valorMaximo = 1000;

        private readonly GeneradorNumerosRandom _generadorNumerosRandom;

        public InstanciaBuilder()
        {
            _generadorNumerosRandom = GeneradorNumerosRandomFactory.Crear();
        }

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

        internal InstanciaBuilder ConValorMaximo(int valorMaximo)
        {
            if (valorMaximo <= 0)
                throw new ArgumentOutOfRangeException(nameof(valorMaximo), $"El valor máximo debe ser positivo: {valorMaximo}");

            _valorMaximo = valorMaximo;
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

            for (int indiceAtomo = 0; indiceAtomo < _cantidadAtomos; indiceAtomo++)
            {
                for (int indiceJugador = 0; indiceJugador < _cantidadJugadores; indiceJugador++)
                {
                    instancia[indiceAtomo][indiceJugador] = _generadorNumerosRandom.Siguiente(0, _valorMaximo + 1);
                }
            }

            return instancia;
        }
    }
}
