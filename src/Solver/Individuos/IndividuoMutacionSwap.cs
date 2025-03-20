using Solver.Fitness;

namespace Solver.Individuos
{
    internal class IndividuoMutacionSwap : Individuo
    {
        internal IndividuoMutacionSwap(List<int> cromosoma, InstanciaProblema problema) : base(cromosoma, problema)
        {
        }

        internal override void Mutar()
        {
            throw new NotImplementedException();
        }

        internal override Individuo Cruzar(Individuo otro)
        {
            throw new NotImplementedException();
        }

        internal override void CalcularFitness(ICalculadoraFitness calculadoraFitness)
        {
            throw new NotImplementedException();
        }
    }
}
