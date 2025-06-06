using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos;

public class IndividuoIntercambioAsignacionesTests : IDisposable
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
        random.SiguienteDouble().Returns(0.0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 1 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(cromosomaOriginal, individuo.Cromosoma);
    }

    [Fact]
    public void Mutar_Cromosoma_MutaCuandoProbabilidadLoPermite()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Solo el primero muta
        random.Siguiente(2).Returns(1); // Dirección +1
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.NotEqual(cromosomaOriginal[0], individuo.Cromosoma[0]);
        Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[1]);
        Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[2]);
    }

    [Fact]
    public void Mutar_MutacionEnCorteSaleDeRangoPorIzquierda_ComportamientoCiclicoPorDerecha()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Mutan los cortes, las asignaciones no
        random.Siguiente(2).Returns(0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(2, individuo.Cromosoma[0]);
    }

    [Fact]
    public void Mutar_MutacionEnCorteSaleDeRangoPorDerecha_ComportamientoCiclicoPorIzquierda()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0); // Mutan los cortes, las asignaciones no
        random.Siguiente(2).Returns(1);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 2, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(0, individuo.Cromosoma[0]);
    }

    [Fact]
    public void Mutar_Asignaciones_MutanCuandoProbabilidadLoPermite()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(1.0, 0.0, 1.0); // Cortes no mutan, asignaciones solo la primera
        random.Siguiente(1, 3).Returns(2); // Intercambia con la posición 2
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = new IndividuoIntercambioAsignaciones([.. cromosomaOriginal], problema, new CalculadoraFitness());

        individuo.Mutar();

        Assert.Equal(cromosomaOriginal[0], individuo.Cromosoma[0]);
        Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[1]);
        Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[2]);
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
        var padre1 = new IndividuoIntercambioAsignaciones([1, 2, 3, 1, 2, 3, 4], problema1, new CalculadoraFitness());

        var problema2 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre2 = new IndividuoIntercambioAsignaciones([1], problema2, new CalculadoraFitness());

        var ex = Assert.Throws<ArgumentException>(() => padre1.Cruzar(padre2));
        Assert.Contains("misma cantidad de cromosomas", ex.Message);
    }

    [Fact]
    public void Cruzar_PadresConUnSoloCromosoma_CreaHijoConUnSoloCromosoma()
    {
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre1 = new IndividuoIntercambioAsignaciones([1], problema, new CalculadoraFitness());
        var padre2 = new IndividuoIntercambioAsignaciones([1], problema, new CalculadoraFitness());

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
        var padre1 = new IndividuoIntercambioAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoIntercambioAsignaciones([3, 3, 3, 2, 3, 4, 1], problema, new CalculadoraFitness());

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
        var padre1 = new IndividuoIntercambioAsignaciones([1, 2, 3, 1, 2, 3, 4], problema, new CalculadoraFitness());
        var padre2 = new IndividuoIntercambioAsignaciones([3, 3, 3, 2, 3, 4, 1], problema, new CalculadoraFitness());

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
        random.Siguiente(1, 4).Returns(1); // Fin del segmento en la posición 1
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

        Assert.Equal([1, 2, 4, 3], hijo.Cromosoma.Skip(3));
    }

    [Fact]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlMedio_CompletaInicioYFinal()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(1); // Inicio del segmento en la posición 1
        random.Siguiente(2, 4).Returns(2); // Fin del segmento en la posición 2
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

        Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
    }

    [Fact]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlFinal_CompletaElInicio()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(3); // Inicio del segmento en la posición 3
        random.Siguiente(4, 4).Returns(4); // Fin del segmento en la posición 4
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

