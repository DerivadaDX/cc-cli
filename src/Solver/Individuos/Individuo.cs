using Solver.Fitness;

namespace Solver.Individuos
{
    internal abstract class Individuo
    {
        internal abstract void Mutar();
        internal abstract Individuo Cruzar(Individuo otro);
        internal abstract void CalcularFitness(ICalculadoraFitness calculadoraFitness);
    }
}
