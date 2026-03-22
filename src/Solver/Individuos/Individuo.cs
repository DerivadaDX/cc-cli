using System;
using System.Collections.Generic;
using Common;

namespace Solver.Individuos
{
    public abstract class Individuo
    {
        protected readonly InstanciaProblema _problema;
        protected readonly GeneradorNumerosRandom _generadorRandom;
        private readonly List<int> _cromosoma;

        protected Individuo(List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        {
            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));

            _cromosoma = cromosoma;
            Cromosoma = _cromosoma.AsReadOnly();
            _problema = problema;
            _generadorRandom = generadorRandom;
        }

        internal IReadOnlyList<int> Cromosoma { get; }

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

        protected int ObtenerGen(int indice)
        {
            int gen = _cromosoma[indice];
            return gen;
        }

        protected void ActualizarGen(int indice, int valor)
        {
            _cromosoma[indice] = valor;
        }

        protected void IntercambiarGenes(int indiceA, int indiceB)
        {
            (_cromosoma[indiceA], _cromosoma[indiceB]) = (_cromosoma[indiceB], _cromosoma[indiceA]);
        }
    }
}
