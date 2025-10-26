using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoOptimizacionAsignacionesTests
    {
        [Fact]
        public void Mutar_Asignaciones_SeAsignaDistribucionOptima()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(1.0);

            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 100m, 1m, 2m, 3m },
                    { 4m, 100m, 5m, 6m },
                    { 7m, 8m, 100m, 9m },
                    { 10m, 11m, 12m, 100m },
                }
            );
            var cromosoma = new List<int> { 1, 2, 3, 4, 3, 2, 1 };
            var individuo = new IndividuoOptimizacionAsignaciones(cromosoma, problema, generadorRandom);

            individuo.Mutar();

            Assert.Equal(1, individuo.Cromosoma[3]);
            Assert.Equal(2, individuo.Cromosoma[4]);
            Assert.Equal(3, individuo.Cromosoma[5]);
            Assert.Equal(4, individuo.Cromosoma[6]);
        }
    }
}
