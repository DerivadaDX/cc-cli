using Common;
using NSubstitute;
using Solver.Individuos;

namespace Solver.Tests.Individuos;

public class IndividuoComunesTests : IDisposable
{
    public static IEnumerable<object[]> TiposIndividuo => new List<object[]>
    {
        new object[] { new Func<List<int>, InstanciaProblema, Individuo>((c,p) => new IndividuoIntercambioAsignaciones([.. c], p, new CalculadoraFitness())) },
        new object[] { new Func<List<int>, InstanciaProblema, Individuo>((c,p) => new IndividuoOptimizacionAsignaciones([.. c], p, new CalculadoraFitness())) }
    };

    public void Dispose()
    {
        GeneradorNumerosRandomFactory.SetearGenerador(null);
        GC.SuppressFinalize(this);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Mutar_CromosomaConUnSoloGen_NoMuta(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 1 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var individuo = creador(new List<int>(cromosomaOriginal), problema);

        individuo.Mutar();

        Assert.Equal(cromosomaOriginal, individuo.Cromosoma);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Mutar_Cromosoma_MutaCuandoProbabilidadLoPermite(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0);
        random.Siguiente(2).Returns(1);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = creador(new List<int>(cromosomaOriginal), problema);

        individuo.Mutar();

        Assert.NotEqual(cromosomaOriginal[0], individuo.Cromosoma[0]);
        Assert.Equal(cromosomaOriginal[1], individuo.Cromosoma[1]);
        Assert.Equal(cromosomaOriginal[2], individuo.Cromosoma[2]);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Mutar_MutacionEnCorteSaleDeRangoPorIzquierda_ComportamientoCiclicoPorDerecha(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0);
        random.Siguiente(2).Returns(0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 0, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = creador(new List<int>(cromosomaOriginal), problema);

        individuo.Mutar();

        Assert.Equal(2, individuo.Cromosoma[0]);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Mutar_MutacionEnCorteSaleDeRangoPorDerecha_ComportamientoCiclicoPorIzquierda(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.SiguienteDouble().Returns(0.0, 1.0);
        random.Siguiente(2).Returns(1);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var cromosomaOriginal = new List<int> { 2, 1, 2 };
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m, 0m }, { 0m, 1m } });
        var individuo = creador(new List<int>(cromosomaOriginal), problema);

        individuo.Mutar();

        Assert.Equal(0, individuo.Cromosoma[0]);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_PadresConDistintaCantidadDeCromosomas_LanzaExcepcion(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var problema1 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema1);

        var problema2 = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre2 = creador(new List<int> { 1 }, problema2);

        var ex = Assert.Throws<ArgumentException>(() => padre1.Cruzar(padre2));
        Assert.Contains("misma cantidad de cromosomas", ex.Message);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_PadresConUnSoloCromosoma_CreaHijoConUnSoloCromosoma(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });
        var padre1 = creador(new List<int> { 1 }, problema);
        var padre2 = creador(new List<int> { 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Single(hijo.Cromosoma);
        Assert.Equal(1, hijo.Cromosoma[0]);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlInicio_QuedanCortesDePadre2(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 2, 3, 4, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([3, 3, 3], hijo.Cromosoma.Take(3));
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlMedio_PrimeraParteDePadre1RestoDePadre2(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(1);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 2, 3, 4, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 3, 3], hijo.Cromosoma.Take(3));
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeCortesPuntoDeCruceAlFinal_QuedanCortesDePadre1(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(2);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 2, 3, 4, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 2, 3], hijo.Cromosoma.Take(3));
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlInicio_CompletaElFinal(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(0);
        random.Siguiente(0, 4).Returns(0);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 4, 3, 2, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 4, 3, 2], hijo.Cromosoma.Skip(3));
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlMedio_CompletaInicioYFinal(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(1);
        random.Siguiente(1, 4).Returns(2);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 4, 3, 2, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal(2, hijo.Cromosoma[4]);
        Assert.Equal(3, hijo.Cromosoma[5]);
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeAsignacionesSegmentoAlFinal_CompletaElInicio(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(3);
        random.Siguiente(3, 4).Returns(3);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 4, 3, 2, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([3, 2, 1, 4], hijo.Cromosoma.Skip(3));
    }

    [Theory]
    [MemberData(nameof(TiposIndividuo))]
    public void Cruzar_SeccionDeAsignacionesSegmentoCompleto_QuedaTodoPadre1(Func<List<int>, InstanciaProblema, Individuo> creador)
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(4).Returns(0);
        random.Siguiente(0, 4).Returns(3);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });
        var padre1 = creador(new List<int> { 1, 2, 3, 1, 2, 3, 4 }, problema);
        var padre2 = creador(new List<int> { 3, 3, 3, 4, 3, 2, 1 }, problema);

        Individuo hijo = padre1.Cruzar(padre2);

        Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
    }
}
