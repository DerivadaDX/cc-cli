using Solver.Fitness;

namespace Solver.Individuos
{
    internal abstract class Individuo
    {
        protected readonly List<int> _cromosoma;
        protected readonly InstanciaProblema _problema;

        protected Individuo(List<int> cromosoma, InstanciaProblema problema)
        {
            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));

            _cromosoma = cromosoma;
            _problema = problema;

            if (cromosoma.Count == 0)
                throw new ArgumentException("El cromosoma no puede estar vacío", nameof(cromosoma));

            ValidarCromosoma(cromosoma, problema);
        }

        public int Fitness { get; protected set; }

        internal abstract void Mutar();
        internal abstract Individuo Cruzar(Individuo otro);
        internal abstract void CalcularFitness(ICalculadoraFitness calculadoraFitness);

        private void ValidarCromosoma(List<int> cromosoma, InstanciaProblema problema)
        {
            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortesEsperada = cantidadAgentes - 1;

            ValidarCantidadGenes(cromosoma, cantidadAgentes);
            ValidarCortes(cromosoma, problema, cantidadCortesEsperada);
            ValidarAsignaciones(cromosoma, cantidadAgentes, cantidadCortesEsperada);
        }

        private void ValidarCantidadGenes(List<int> cromosoma, int cantidadAgentes)
        {
            int cantidadCortesEsperada = cantidadAgentes - 1;
            int cantidadGenesEsperada = cantidadCortesEsperada + cantidadAgentes;

            if (cromosoma.Count != cantidadGenesEsperada)
            {
                string mensaje = $"Cantidad de genes inválida. Esperada: {cantidadGenesEsperada}, recibida: {cromosoma.Count}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void ValidarCortes(List<int> cromosoma, InstanciaProblema problema, int cantidadCortesEsperada)
        {
            List<int> cortes = [.. cromosoma.Take(cantidadCortesEsperada).Order()];
            if (cortes.First() < 0)
            {
                string mensaje = $"El primer corte no puede ser negativo: {cortes.First()}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            if (cortes.Last() > problema.CantidadAtomos)
            {
                string mensajeTemplate = "El último corte no puede superar la cantidad de átomos ({0}): {1}";
                string mensaje = string.Format(mensajeTemplate, problema.CantidadAtomos, cortes.Last());
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void ValidarAsignaciones(List<int> cromosoma, int cantidadAgentes, int cantidadCortesEsperada)
        {
            List<int> asignaciones = [.. cromosoma.Skip(cantidadCortesEsperada)];
            List<int> fueraDeRango = [.. asignaciones.Where(a => a < 1 || a > cantidadAgentes).Distinct().Order()];
            if (fueraDeRango.Count > 0)
            {
                string mensaje = $"Hay asignaciones fuera de rango [1, {cantidadAgentes}]: ({string.Join(", ", fueraDeRango)})";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            List<int> repetidas = asignaciones.GroupBy(a => a).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (repetidas.Count > 0)
            {
                string mensaje = $"Hay asignaciones repetidas a agentes: ({string.Join(", ", repetidas)})";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }
    }
}
