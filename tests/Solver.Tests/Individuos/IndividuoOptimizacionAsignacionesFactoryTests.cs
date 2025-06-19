using Solver.Individuos;

namespace Solver.Tests.Individuos;

public class IndividuoOptimizacionAsignacionesFactoryTests
{
    [Fact]
    public void CrearAleatorio_InstanciaDevuelta_EsDelTipoCorrecto()
    {
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m },
            { 0m, 1m },
        });

        var individuoFactory = new IndividuoOptimizacionAsignacionesFactory(problema);
        Individuo individuo = individuoFactory.CrearAleatorio();

        Assert.IsType<IndividuoOptimizacionAsignaciones>(individuo);
    }

    [Fact]
    public void CrearAleatorio_CromosomaDeInstanciaDevuelta_LongitudCorrecta()
    {
        // 3 tomos, 5 agentes
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m, 1m },
            { 0m, 1m, 0m, 0m, 0m },
            { 0m, 0m, 1m, 1m, 0m },
        });

        var individuoFactory = new IndividuoOptimizacionAsignacionesFactory(problema);
        Individuo individuo = individuoFactory.CrearAleatorio();

        int longitudCromosomaEsperada = 9; // 4 cortes + 5 asignaciones
        Assert.Equal(longitudCromosomaEsperada, individuo.Cromosoma.Count);
    }

    [Fact]
    public void CrearAleatorio_CortesDeInstanciaDevuelta_DentroDelRango()
    {
        // 3 tomos, 5 agentes
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m, 1m },
            { 0m, 1m, 0m, 0m, 0m },
            { 0m, 0m, 1m, 1m, 0m },
        });

        var individuoFactory = new IndividuoOptimizacionAsignacionesFactory(problema);
        Individuo individuo = individuoFactory.CrearAleatorio();

        int cantidadCortes = 4;
        var cortes = individuo.Cromosoma.Take(cantidadCortes).ToList<int>();

        Assert.All(cortes, c => Assert.InRange(c, 0, problema.CantidadAtomos));
    }

    [Fact]
    public void CrearAleatorio_AsignacionesDeInstanciaDevuelta_DentroDelRango()
    {
        // 3 tomos, 5 agentes
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m, 1m },
            { 0m, 1m, 0m, 0m, 0m },
            { 0m, 0m, 1m, 1m, 0m },
        });

        var individuoFactory = new IndividuoOptimizacionAsignacionesFactory(problema);
        Individuo individuo = individuoFactory.CrearAleatorio();

        int cantidadCortes = 4;
        int cantidadAgentes = 5;
        var asignaciones = individuo.Cromosoma.Skip(cantidadCortes).ToList<int>();

        Assert.Equal(cantidadAgentes, asignaciones.Count);
        Assert.Equal(cantidadAgentes, asignaciones.Distinct().Count());
        Assert.All(asignaciones, a => Assert.InRange(a, 1, cantidadAgentes));
    }
}
