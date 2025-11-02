using Solver;

public class IndividuoNuevoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_CantidadAtomosInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new IndividuoNuevo(cantidadAtomos, cantidadJugadores: 1));
        Assert.Contains("debe ser mayor o igual a 1", ex.Message);
        Assert.Equal("cantidadAtomos", ex.ParamName);
    }

    [Fact]
    public void Constructor_CantidadJugadoresMayorQueCantidadAtomos_LanzaArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new IndividuoNuevo(cantidadAtomos: 5, cantidadJugadores: 6));
        Assert.Contains("no puede ser mayor que la cantidad de átomos", ex.Message);
        Assert.Equal("cantidadJugadores", ex.ParamName);
    }

    [Fact]
    public void Constructor_Cromosoma_TamañoCorrecto()
    {
        int cantidadAtomos = 5;
        var individuo = new IndividuoNuevo(cantidadAtomos, cantidadJugadores: 1);

        Assert.NotNull(individuo.Cromosoma);
        Assert.Equal(cantidadAtomos - 1, individuo.Cromosoma.Count);
    }

    [Fact]
    public void Contructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
    {
        int cantidadAtomos = 5;
        int cantidadJugadores = 3;
        var individuo = new IndividuoNuevo(cantidadAtomos, cantidadJugadores);

        int cantidadUnosEsperada = cantidadJugadores - 1;
        int cantidadUnos = individuo.Cromosoma.Count(gen => gen == 1);
        Assert.Equal(cantidadUnosEsperada, cantidadUnos);

        int cantidadCerosEsperada = cantidadAtomos - 1 - cantidadUnosEsperada;
        int cantidadCeros = individuo.Cromosoma.Count(gen => gen == 0);
        Assert.Equal(cantidadCerosEsperada, cantidadCeros);

        Assert.Equal(cantidadUnosEsperada + cantidadCerosEsperada, individuo.Cromosoma.Count);
    }
}
