using Common;

namespace Solver
{
    public class IndividuoNuevo
    {
        private readonly GeneradorNumerosRandom _generadorRandom;
        private readonly List<int> _cromosoma;
        private readonly List<int> _asignaciones;

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

            int tamañoCromosoma = cantidadAtomos - 1;
            _cromosoma = [.. new int[tamañoCromosoma]];
            InicializarCromosomaAleatorio(cantidadUnos: cantidadAgentes - 1);

            // Falta calcular la matriz de valoraciones de porciones.
            var algoritmoHungaro = AlgoritmoHungaroFactory.Crear();
            var valoracionesDePorciones = new decimal[cantidadAtomos, cantidadAgentes];
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
