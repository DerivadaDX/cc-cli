using Common;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class CalculadoraFitnessFactoryTests : IDisposable
    {
        public void Dispose()
        {
            CalculadoraFitnessFactory.SetearCalculadora(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            var calculadora = CalculadoraFitnessFactory.Crear();
            Assert.NotNull(calculadora);
            Assert.IsType<CalculadoraFitness>(calculadora);
        }

        [Fact]
        public void SetearCalculadora_Calculadora_SeSeteaCorrectamente()
        {
            var calculadora1 = new CalculadoraFitness();
            CalculadoraFitnessFactory.SetearCalculadora(calculadora1);

            var calculadora2 = CalculadoraFitnessFactory.Crear();

            Assert.Same(calculadora1, calculadora2);
        }
    }
}
