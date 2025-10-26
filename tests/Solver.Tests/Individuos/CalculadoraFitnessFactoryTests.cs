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
            var calculadoraSeteada = new CalculadoraFitness();
            CalculadoraFitnessFactory.SetearCalculadora(calculadoraSeteada);

            var calculadoraObtenida = CalculadoraFitnessFactory.Crear();

            Assert.Same(calculadoraSeteada, calculadoraObtenida);
        }
    }
}
