using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoIntercambioAsignacionesTests : IDisposable
    {
        public void Dispose()
        {
            GeneradorNumerosRandomFactory.SetearGenerador(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Mutar_Asignaciones_MutanCuandoProbabilidadLoPermite()
        {
            var random = Substitute.For<GeneradorNumerosRandom>(1);
            random.SiguienteDouble().Returns(1.0, 0.0, 1.0); // Cortes no mutan, asignaciones solo la primera
            random.Siguiente(1, 3).Returns(2); // Intercambia con la posición 2
            GeneradorNumerosRandomFactory.SetearGenerador(random);

            var cromosomaOriginal = new List<int> { 0, 1, 2 };
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(
                new decimal[,]
                {
                    { 1m, 0m },
                    { 0m, 1m },
                }
            );
            var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema);

            individuo.Mutar();

            Assert.Equal(cromosomaOriginal[0], individuo.Cromosoma[0]);
            Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[1]);
            Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[2]);
        }
    }
}

