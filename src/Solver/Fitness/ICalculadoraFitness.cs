using Solver.Individuos;

namespace Solver.Fitness
{
    internal interface ICalculadoraFitness
    {
        decimal CalcularFitness(Individuo individuo, InstanciaProblema instanciaProblema);
    }
}
