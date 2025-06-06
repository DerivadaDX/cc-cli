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
    public void Cruzar_PuntoDeCorteInicial_CortesCombinados()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(1);
        random.Siguiente(4).Returns(0);
        random.Siguiente(1, 4).Returns(2);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });

        var padre1 = new IndividuoStub([1, 2, 3, 1, 2, 3, 4], problema);
        var padre2 = new IndividuoStub([3, 3, 3, 2, 3, 4, 1], problema);

        var hijo = (IndividuoStub)padre1.Cruzar(padre2);
        
        Assert.Equal([1, 3, 3], hijo.Cromosoma.Take(3));
        Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
    }

    [Fact]
    public void Cruzar_PuntoDeCorteFinal_CortesCombinados()
    {
        var random = Substitute.For<GeneradorNumerosRandom>();
        random.Siguiente(1, 3).Returns(2);
        random.Siguiente(4).Returns(1);
        random.Siguiente(2, 4).Returns(3);
        GeneradorNumerosRandomFactory.SetearGenerador(random);

        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
        {
            { 1m, 0m, 0m, 0m },
            { 0m, 1m, 0m, 0m },
            { 0m, 0m, 1m, 0m },
            { 0m, 0m, 0m, 1m },
        });

        var padre1 = new IndividuoStub([1, 2, 3, 1, 2, 3, 4], problema);
        var padre2 = new IndividuoStub([3, 4, 4, 4, 3, 2, 1], problema);

        var hijo = (IndividuoStub)padre1.Cruzar(padre2);

        Assert.Equal([1, 2, 4], hijo.Cromosoma.Take(3));
        Assert.Equal([1, 2, 3, 4], hijo.Cromosoma.Skip(3));
    }

    [Fact]
    public void Cruzar_CromosomaUnAgente_NoLanzaExcepcion()
    {
        var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1m } });

        var padre1 = new IndividuoStub([1], problema);
        var padre2 = new IndividuoStub([1], problema);

        var hijo = padre1.Cruzar(padre2);

        var cromosomaHijo = ((IndividuoStub)hijo).Cromosoma;
        Assert.Single(cromosomaHijo);
        Assert.Equal(1, cromosomaHijo[0]);
    }

    private class IndividuoStub : IndividuoIntercambioAsignaciones
    {
        internal IndividuoStub(List<int> cromosoma, InstanciaProblema problema)
            : base(cromosoma, problema, new CalculadoraFitness())
        {
        }

        internal override Individuo Cruzar(Individuo otro)
        {
            var baseHijo = (IndividuoIntercambioAsignaciones)base.Cruzar(otro);
            var field = typeof(Individuo).GetField("_cromosoma", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var lista = (List<int>)field!.GetValue(baseHijo)!;
            return new IndividuoStub(lista.ToList(), _problema);
        }

        internal List<int> Cromosoma => _cromosoma;
    }
}

