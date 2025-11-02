using Solver;

public class IndividuoNuevoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_CantidadAtomosInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new IndividuoNuevo(cantidadAtomos));
        Assert.Contains("debe ser mayor o igual a 1", ex.Message);
        Assert.Equal("cantidadAtomos", ex.ParamName);
    }

    [Fact]
    public void Constructor_Cromosoma_TamañoCorrecto()
    {
        int cantidadAtomos = 5;
        var individuo = new IndividuoNuevo(cantidadAtomos);

        Assert.NotNull(individuo.Cromosoma);
        Assert.Equal(cantidadAtomos - 1, individuo.Cromosoma.Count);
    }
}
