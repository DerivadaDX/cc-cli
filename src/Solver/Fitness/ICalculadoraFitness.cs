using Solver.Individuos;

namespace Solver.Fitness
{
    internal interface ICalculadoraFitness
    {
        int CalcularFitness(Individuo individuo, InstanciaProblema instanciaProblema);
    }
}
