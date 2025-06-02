using Common;
using Solver.Individuos;

namespace Solver
{
    internal class Poblacion
    {
        private readonly GeneradorNumerosRandom _random;
        private readonly int _tamaño;

        public Poblacion(int tamaño)
        {
            if (tamaño <= 0)
            {
                string mensaje = $"El tamaño de la población debe ser mayor a cero: {tamaño}";
                throw new ArgumentOutOfRangeException(nameof(tamaño), mensaje);
            }

            _random = GeneradorNumerosRandomFactory.Crear();
            _tamaño = tamaño;
        }

        internal virtual List<Individuo> Individuos { get; private set; } = [];

        internal virtual Poblacion GenerarNuevaGeneracion()
        {
            var nuevaGeneracion = new Poblacion(_tamaño);

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
            Individuo resultado = Individuos.OrderBy(individuo => individuo.Fitness).FirstOrDefault();
            return resultado;
        }

        private List<Individuo> SeleccionarElite()
        {
            const decimal FraccionElite = 0.01M;
            int cantidadElite = (int)Math.Ceiling(Individuos.Count * FraccionElite);
            List<Individuo> elite = [.. Individuos.OrderBy(individuo => individuo.Fitness).Take(cantidadElite)];
            return elite;
        }

        private Individuo SeleccionarIndividuoPorTorneo()
        {
            int indice1 = _random.Siguiente(0, Individuos.Count);
            int indice2 = _random.Siguiente(0, Individuos.Count);

            Individuo resultado = Individuos[indice1].Fitness < Individuos[indice2].Fitness
                ? Individuos[indice1] : Individuos[indice2];

            return resultado;
        }
    }
}
