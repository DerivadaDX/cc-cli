using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos;

public class IndividuoOptimizacionAsignacionesTests : IDisposable
{
    public void Dispose()
    {
        GeneradorNumerosRandomFactory.SetearGenerador(null);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Mutar_CromosomaConUnSoloGen_NoMuta()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(1.0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 1 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(cromosomaOriginal, individuo.Cromosoma);
    }

    [Fact]
    public void Mutar_Asignaciones_SeAsignaDistribucionOptima()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(1.0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        // Dos atomos, dos agentes
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m },
            { 0m, 1m },
        });
        var cromosoma = new List<int> { 1, 2, 1 }; // corte en 1, asignaciones invertidas
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosoma], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(1, individuo.Cromosoma[1]);
        Assert.Equal(2, individuo.Cromosoma[2]);
    }

    [Fact]
    public void Cruzar_PadresConUnSoloCromosoma_CreaHijoConUnSoloCromosoma()
    {
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre1 = new IndividuoOptimizacionAsignaciones([1], problema, new CalculadoraFitness());
        var padre2 = new IndividuoOptimizacionAsignaciones([1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Single(hijo.Cromosoma);
        Assert.Equal(1, hijo.Cromosoma[0]);
    }
}
