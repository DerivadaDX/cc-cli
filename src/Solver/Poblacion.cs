using Common;
using Solver.Individuos;

namespace Solver
{
    public class Poblacion
    {
        private readonly int _tamaño;
        private readonly GeneradorNumerosRandom _generadorRandom;

        public Poblacion(int tamaño, GeneradorNumerosRandom generadorRandom)
        {
            if (tamaño <= 0)
            {
                string mensaje = $"El tamaño de la población debe ser mayor a cero: {tamaño}";
                throw new ArgumentOutOfRangeException(nameof(tamaño), mensaje);
            }

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));

            _generadorRandom = generadorRandom;
            _tamaño = tamaño;
        }

        internal virtual List<Individuo> Individuos { get; private set; } = [];

        internal virtual Poblacion GenerarNuevaGeneracion()
        {
            var nuevaGeneracion = new Poblacion(_tamaño, _generadorRandom);

            List<Individuo> elite = SeleccionarElite();
            nuevaGeneracion.Individuos.AddRange(elite);

            while (nuevaGeneracion.Individuos.Count < _tamaño)
            {
                Individuo padre1 = SeleccionarIndividuoPorTorneo();
                Individuo padre2 = SeleccionarIndividuoPorTorneo();
                Individuo hijo = padre1.Cruzar(padre2);
                hijo.Mutar();
                nuevaGeneracion.Individuos.Add(hijo);
            }

            return nuevaGeneracion;
        }

        internal virtual Individuo ObtenerMejorIndividuo()
        {
            Individuo resultado = Individuos.OrderBy(individuo => individuo.Fitness()).FirstOrDefault();
            return resultado;
        }

        private List<Individuo> SeleccionarElite()
        {
            // Fracción de la población que se selecciona como individuos élite.
            // Nota: Para poblaciones menores a 100, este cálculo siempre selecciona al menos un individuo élite
            // debido al uso de Math.Ceiling. Esto asegura que el individuo con mejor desempeño se conserve entre generaciones.
            const decimal FraccionElite = 0.01M;
            int cantidadElite = (int)Math.Ceiling(Individuos.Count * FraccionElite);
            List<Individuo> elite = [.. Individuos.OrderBy(individuo => individuo.Fitness()).Take(cantidadElite)];
            return elite;
        }

        private Individuo SeleccionarIndividuoPorTorneo()
        {
            int indice1 = _generadorRandom.Siguiente(Individuos.Count);
            Individuo individuo1 = Individuos[indice1];

            int indice2 = _generadorRandom.Siguiente(Individuos.Count);
            Individuo individuo2 = Individuos[indice2];

            Individuo resultado = individuo1.Fitness() <= individuo2.Fitness() ? individuo1 : individuo2;
            return resultado;
        }
    }
}
