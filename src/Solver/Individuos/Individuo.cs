using System.Globalization;

namespace Solver.Individuos
{
    public abstract class Individuo
    {
        protected readonly InstanciaProblema _problema;
        protected readonly CalculadoraFitness _calculadoraFitness;

        protected Individuo(List<int> cromosoma, InstanciaProblema problema, CalculadoraFitness calculadoraFitness)
        {
            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ArgumentNullException.ThrowIfNull(calculadoraFitness, nameof(calculadoraFitness));

            Cromosoma = cromosoma;
            _problema = problema;
            _calculadoraFitness = calculadoraFitness;

            if (cromosoma.Count == 0)
                throw new ArgumentException("El cromosoma no puede estar vacío", nameof(cromosoma));

            ValidarCromosoma(cromosoma, problema);
        }

        internal abstract void Mutar();
        internal abstract Individuo Cruzar(Individuo otro);

        internal virtual List<int> Cromosoma { get; }

        public override string ToString()
        {
            string cromosomaStr = string.Join(", ", Cromosoma);
            decimal fitness = Fitness();
            return $"Cromosoma: [{cromosomaStr}], Fitness: {fitness.ToString(CultureInfo.InvariantCulture)}";
        }

        internal virtual decimal Fitness()
        {
            decimal fitness = _calculadoraFitness.CalcularFitness(this, _problema);
            return fitness;
        }

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

            if (cantidadCortesEsperada == 0 || cortes.Count == 0)
                return;

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
