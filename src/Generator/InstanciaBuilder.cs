using Common;

namespace Generator
{
    public class InstanciaBuilder
    {
        private int _cantidadAtomos;
        private int _cantidadAgentes;
        private int _valorMaximo;
        private bool? _valoracionesDisjuntas;

        private readonly GeneradorNumerosRandom _generadorNumerosRandom;

        public InstanciaBuilder(GeneradorNumerosRandom generadorNumerosRandom)
        {
            ArgumentNullException.ThrowIfNull(generadorNumerosRandom, nameof(generadorNumerosRandom));
            _generadorNumerosRandom = generadorNumerosRandom;
        }

        public virtual InstanciaBuilder ConCantidadDeAtomos(int cantidadAtomos)
        {
            if (cantidadAtomos <= 0)
            {
                string mensaje = $"La cantidad de átomos debe ser mayor que cero: {cantidadAtomos}";
                throw new ArgumentOutOfRangeException(nameof(cantidadAtomos), mensaje);
            }

            _cantidadAtomos = cantidadAtomos;
            return this;
        }

        public virtual InstanciaBuilder ConCantidadDeAgentes(int cantidadAgentes)
        {
            if (cantidadAgentes <= 0)
            {
                string mensaje = $"La cantidad de agentes debe ser mayor que cero: {cantidadAgentes}";
                throw new ArgumentOutOfRangeException(nameof(cantidadAgentes), mensaje);
            }

            _cantidadAgentes = cantidadAgentes;
            return this;
        }

        public virtual InstanciaBuilder ConValorMaximo(int valorMaximo)
        {
            if (valorMaximo <= 0)
                throw new ArgumentOutOfRangeException(nameof(valorMaximo), $"El valor máximo debe ser positivo: {valorMaximo}");

            _valorMaximo = valorMaximo;
            return this;
        }

        public virtual InstanciaBuilder ConValoracionesDisjuntas(bool valoracionesDisjuntas)
        {
            _valoracionesDisjuntas = valoracionesDisjuntas;
            return this;
        }

        public virtual decimal[,] Build()
        {
            if (_cantidadAtomos == 0)
                throw new InvalidOperationException("Debe especificar el número de átomos antes de construir la instancia");

            if (_cantidadAgentes == 0)
                throw new InvalidOperationException("Debe especificar el número de agentes antes de construir la instancia");

            if (_valorMaximo == 0)
                throw new InvalidOperationException("Debe especificar el valor máximo antes de construir la instancia");

            if (!_valoracionesDisjuntas.HasValue)
                throw new InvalidOperationException("Debe especificar el tipo (disjunta o no disjunta) antes de construir la instancia");

            decimal[,] instancia;

            if (_valoracionesDisjuntas.Value)
                instancia = ConstruirInstanciaDisjunta();
            else
                instancia = ConstruirInstanciaNoDisjunta();

            return instancia;
        }

        private decimal[,] ConstruirInstanciaDisjunta()
        {
            if (_cantidadAtomos < _cantidadAgentes)
            {
                string mensaje = "No se puede generar una instancia con más agentes que átomos si las valoraciones son disjuntas";
                throw new InvalidOperationException(mensaje);
            }

            var instancia = new decimal[_cantidadAtomos, _cantidadAgentes];
            var atomosDisponibles = Enumerable.Range(0, _cantidadAtomos).ToList<int>();
            var agentesDisponibles = Enumerable.Range(0, _cantidadAgentes).ToList<int>();

            while (agentesDisponibles.Count > 0)
            {
                int indiceAtomo = _generadorNumerosRandom.Siguiente(atomosDisponibles.Count);
                int indiceAgente = _generadorNumerosRandom.Siguiente(agentesDisponibles.Count);

                int atomoElegido = atomosDisponibles[indiceAtomo];
                int agenteElegido = agentesDisponibles[indiceAgente];
                instancia[atomoElegido, agenteElegido] = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);

                atomosDisponibles.RemoveAt(indiceAtomo);
                agentesDisponibles.RemoveAt(indiceAgente);
            }

            while (atomosDisponibles.Count > 0)
            {
                int indiceAtomo = _generadorNumerosRandom.Siguiente(atomosDisponibles.Count);
                int agenteElegido = _generadorNumerosRandom.Siguiente(_cantidadAgentes);
                int atomoElegido = atomosDisponibles[indiceAtomo];

                instancia[atomoElegido, agenteElegido] = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);

                atomosDisponibles.RemoveAt(indiceAtomo);
            }

            return instancia;
        }

        private decimal[,] ConstruirInstanciaNoDisjunta()
        {
            var instancia = new decimal[_cantidadAtomos, _cantidadAgentes];

            for (int indiceAtomo = 0; indiceAtomo < _cantidadAtomos; indiceAtomo++)
            {
                for (int indiceAgente = 0; indiceAgente < _cantidadAgentes; indiceAgente++)
                {
                    int valorAleatorio = _generadorNumerosRandom.Siguiente(1, _valorMaximo + 1);
                    instancia[indiceAtomo, indiceAgente] = valorAleatorio;
                }
            }

            return instancia;
        }
    }
}
