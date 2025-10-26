using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoIntercambioAsignacionesTests
    {
        [Fact]
        public void Mutar_Asignaciones_MutanCuandoProbabilidadLoPermite()
        {
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);
            generadorRandom.SiguienteDouble().Returns(1.0, 0.0, 1.0); // Cortes no mutan, asignaciones solo la primera
            generadorRandom.Siguiente(1, 3).Returns(2); // Intercambia con la posición 2

            var cromosomaOriginal = new List<int> { 0, 1, 2 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, generadorRandom);

            individuo.Mutar();

            Assert.Equal(cromosomaOriginal[0], individuo.Cromosoma[0]);
            Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[1]);
            Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[2]);
        }
    }
}
