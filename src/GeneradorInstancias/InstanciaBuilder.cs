
using Common;

namespace GeneradorInstancias
{
    internal class InstanciaBuilder
    {
        private int _valorMaximo = 1000;
        private bool _valoracionesDisjuntas = false;

        private int _cantidadAtomos;
        private int _cantidadJugadores;

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

        internal InstanciaBuilder ConValoracionesDisjuntas(bool valoracionesDisjuntas)
        {
            _valoracionesDisjuntas = valoracionesDisjuntas;
            return this;
        }

        internal decimal[][] Build()
        {
            decimal[][] instancia;

            if (_valoracionesDisjuntas)
                instancia = ConstruirInstanciaDisjunta();
            else
                instancia = ConstruirInstanciaNoDisjunta();

            return instancia;
        }

        private decimal[][] ConstruirInstanciaDisjunta()
        {
            if (_cantidadAtomos < _cantidadJugadores)
            {
                string mensaje = "No se puede generar una instancia con más jugadores que átomos si las valoraciones son disjuntas";
                throw new InvalidOperationException(mensaje);
            }

            decimal[][] instancia = ConstruirInstanciaVacia();

            List<int> atomosDisponibles = Enumerable.Range(0, _cantidadAtomos).ToList();
            List<int> jugadoresDisponibles = Enumerable.Range(0, _cantidadJugadores).ToList();

            while (jugadoresDisponibles.Count > 0)
            {
                int indiceAtomo = _generadorNumerosRandom.Siguiente(0, atomosDisponibles.Count);
                int atomoElegido = atomosDisponibles[indiceAtomo];

                int indiceJugador = _generadorNumerosRandom.Siguiente(0, jugadoresDisponibles.Count);
                int jugadorElegido = jugadoresDisponibles[indiceJugador];

                instancia[atomoElegido][jugadorElegido] = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);

                atomosDisponibles.RemoveAt(indiceAtomo);
                jugadoresDisponibles.RemoveAt(indiceJugador);
            }

            while (atomosDisponibles.Count > 0)
            {
                int indiceAtomo = _generadorNumerosRandom.Siguiente(0, atomosDisponibles.Count);
                int jugadorElegido = _generadorNumerosRandom.Siguiente(0, _cantidadJugadores);
                int atomoElegido = atomosDisponibles[indiceAtomo];

                instancia[atomoElegido][jugadorElegido] = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);

                atomosDisponibles.RemoveAt(indiceAtomo);
            }

            return instancia;
        }

        private decimal[][] ConstruirInstanciaNoDisjunta()
        {
            decimal[][] instancia = ConstruirInstanciaVacia();

            for (int indiceAtomo = 0; indiceAtomo < _cantidadAtomos; indiceAtomo++)
            {
                for (int indiceJugador = 0; indiceJugador < _cantidadJugadores; indiceJugador++)
                {
                    int valorAleatorio = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);
                    instancia[indiceAtomo][indiceJugador] = valorAleatorio;
                }
            }

            return instancia;
        }

        private decimal[][] ConstruirInstanciaVacia()
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
