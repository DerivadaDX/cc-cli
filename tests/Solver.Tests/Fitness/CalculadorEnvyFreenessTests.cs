using Solver.Fitness;
using Solver.Individuos;

namespace Solver.Tests.Fitness
{
    public class CalculadorEnvyFreenessTests
    {
        private readonly CalculadorEnvyFreeness _calculador = new();

        [Fact]
        public void CalcularFitness_AsignacionSinEnvidia_RetornaCero()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m }
            });
            var cromosoma = new List<int> { 1, 1, 2 };
            var individuo = new IndividuoStub(cromosoma, problema);

            decimal fitness = _calculador.CalcularFitness(individuo, problema);
            Assert.Equal(0, fitness);
        }

        [Fact]
        public void CalcularFitness_HayEnvidia_RetornaPositivo()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, .5m },
                { 0m, .5m },
            });
            var cromosoma = new List<int> { 1, 2, 1 };
            var individuo = new IndividuoStub(cromosoma, problema);

            decimal fitness = _calculador.CalcularFitness(individuo, problema);
            Assert.True(fitness > 0);
        }

        [Fact]
        public void CalcularFitness_TodosValoracionesIguales_SinEnvidia()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { .5m, .5m },
                { .5m, .5m },
            });

            var cromosoma = new List<int> { 1, 1, 2 };
            var individuo = new IndividuoStub(cromosoma, problema);

            decimal fitness = _calculador.CalcularFitness(individuo, problema);
            Assert.Equal(0, fitness);
        }

        private class IndividuoStub : Individuo
        {
            internal IndividuoStub(List<int> cromosoma, InstanciaProblema problema) : base(cromosoma, problema)
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
}