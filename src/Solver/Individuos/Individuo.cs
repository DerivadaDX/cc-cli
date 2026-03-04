using Common;

namespace Solver.Individuos
{
    public abstract class Individuo
    {
        protected readonly InstanciaProblema _problema;
        protected readonly GeneradorNumerosRandom _generadorRandom;

        protected Individuo(List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        {
            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));

            Cromosoma = cromosoma;
            _problema = problema;
            _generadorRandom = generadorRandom;
        }

        internal virtual List<int> Cromosoma { get; }

        protected abstract string FamiliaCromosoma { get; }

        internal abstract void Mutar();

        internal abstract Individuo Cruzar(Individuo otro);

        internal abstract decimal Fitness();

        protected void ValidarCompatibilidadCruce(Individuo otro)
        {
            ArgumentNullException.ThrowIfNull(otro, nameof(otro));

            if (FamiliaCromosoma != otro.FamiliaCromosoma)
                throw new InvalidOperationException("No se puede cruzar individuos de familias de cromosoma diferentes.");
        }
    }
}
