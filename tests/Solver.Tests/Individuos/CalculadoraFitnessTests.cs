using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class CalculadoraFitnessTests
    {
        private readonly CalculadoraFitness _calculadora = new();

        [Fact]
        public void CalcularFitness_AsignacionSinEnvidia_RetornaCero()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m }
            });
            var cromosoma = new List<int> { 1, 1, 2 };
            var individuo = new IndividuoFake(cromosoma, problema);

            decimal fitness = _calculadora.CalcularFitness(individuo, problema);
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
            var individuo = new IndividuoFake(cromosoma, problema);

            decimal fitness = _calculadora.CalcularFitness(individuo, problema);
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
            var individuo = new IndividuoFake(cromosoma, problema);

            decimal fitness = _calculadora.CalcularFitness(individuo, problema);
            Assert.Equal(0, fitness);
        }

        private class IndividuoFake : Individuo
        {
            internal IndividuoFake(List<int> cromosoma, InstanciaProblema problema)
                : base(cromosoma, problema, new CalculadoraFitness())
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
        }
    }
}