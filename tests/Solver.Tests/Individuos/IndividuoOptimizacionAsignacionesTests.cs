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
    public void Mutar_Cromosoma_MutaCorteCuandoProbabilidadLoPermite()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Solo el primero muta
        random.Siguiente(2).Returns(1); // Dirección +1
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.NotEqual(cromosomaOriginal[0], individuo.Cromosoma[0]);
        Assert.Equal(1, individuo.Cromosoma[0]);
    }

    [Fact]
    public void Mutar_MutacionEnCorteSaleDeRangoPorIzquierda_ComportamientoCiclicoPorDerecha()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Solo el primer corte muta
        random.Siguiente(2).Returns(0); // Dirección -1
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(2, individuo.Cromosoma[0]);
    }

    [Fact]
    public void Mutar_MutacionEnCorteSaleDeRangoPorDerecha_ComportamientoCiclicoPorIzquierda()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Solo el primer corte muta
        random.Siguiente(2).Returns(1); // Dirección +1
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 2, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(0, individuo.Cromosoma[0]);
    }

    [Fact]
    public void Mutar_Asignaciones_SeAsignaDistribucionOptima()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(1.0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 100m, 1m, 2m, 3m },
            { 4m, 100m, 5m, 6m },
            { 7m, 8m, 100m, 9m },
            { 10m, 11m, 12m, 100m },
        });
        var cromosoma = new List<int> { 1, 2, 3, 4, 3, 2, 1 };
        var individuo = new IndividuoOptimizacionAsignaciones([.. cromosoma], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(1, individuo.Cromosoma[3]);
        Assert.Equal(2, individuo.Cromosoma[4]);
        Assert.Equal(3, individuo.Cromosoma[5]);
        Assert.Equal(4, individuo.Cromosoma[6]);
    }

    [Fact]
    public void Cruzar_PadresConDistintaCantidadDeCromosomas_LanzaExcepcion()
    {
        var problema1 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoOptimizacionAsignaciones([1, 2, 3, 1, 2, 3, 4], problema1, new CalculadoraFitness());

        var problema2 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre2 = new IndividuoOptimizacionAsignaciones([1], problema2, new CalculadoraFitness());

        var ex = Assert.Throws<ArgumentException>(() => padre1.Cruzar(padre2));
        Assert.Contains("misma cantidad de cromosomas", ex.Message);
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

    [Fact]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlInicio_QuedanCortesDePadre2()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(0); // Punto de corte en la posición 0
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoOptimizacionAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoOptimizacionAsignaciones([3, 3, 3, 2, 3, 4, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([3, 3, 3], hijo.Cromosoma.Take(3));
    }

    [Fact]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlMedio_PrimeraParteDePadre1RestoDePadre2()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(1); // Punto de corte en la posición 1
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoOptimizacionAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoOptimizacionAsignaciones([3, 3, 3, 2, 3, 4, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 3, 3], hijo.Cromosoma.Take(3));
    }

    [Fact]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlFinal_QuedanCortesDePadre1()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(2); // Punto de corte en la posición 2
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoIntercambioAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoIntercambioAsignaciones([3, 3, 3, 2, 3, 4, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 2, 3], hijo.Cromosoma.Take(3));
    }

    [Fact]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlInicio_CompletaElFinal()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(0); // Inicio del segmento en la posición 0
        random.Siguiente(0, 4).Returns(0); // Fin del segmento en la posición 0
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoOptimizacionAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoOptimizacionAsignaciones([3, 3, 3, 4, 3, 2, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        // Primero está el segmento del padre1, luego los que no aparecen del padre2
        Assert.Equal([1, 4, 3, 2], hijo.Cromosoma.Skip(3));
    }

    [Fact]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlMedio_CompletaInicioYFinal()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(1); // Inicio del segmento en la posición 1
        random.Siguiente(1, 4).Returns(2); // Fin del segmento en la posición 2
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoOptimizacionAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoOptimizacionAsignaciones([3, 3, 3, 4, 3, 2, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal(2, hijo.Cromosoma[4]);
        Assert.Equal(3, hijo.Cromosoma[5]);
    }

    [Fact]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlFinal_CompletaElInicio()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(3); // Inicio del segmento en la posición 3
        random.Siguiente(3, 4).Returns(3); // Fin del segmento en la posición 3
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = new IndividuoIntercambioAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoIntercambioAsignaciones([3, 3, 3, 4, 3, 2, 1], problema, new CalculadoraFitness());

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([3, 2, 1, 4], hijo.Cromosoma.Skip(3));
    }
}
