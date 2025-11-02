using Common;
using NSubstitute;
using Solver;

public class IndividuoNuevoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_CantidadAtomosInvalida_LanzaArgumentOutOfRangeException(int cantidadAtomos)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CrearIndividuo(cantidadAtomos, cantidadAgentes: 1));
        Assert.Contains("debe ser mayor o igual a 1", ex.Message);
        Assert.Equal("cantidadAtomos", ex.ParamName);
    }

    [Fact]
    public void Constructor_CantidadJugadoresMayorQueCantidadAtomos_LanzaArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CrearIndividuo(cantidadAtomos: 5, cantidadAgentes: 6));
        Assert.Contains("no puede ser mayor que la cantidad de átomos", ex.Message);
        Assert.Equal("cantidadJugadores", ex.ParamName);
    }

    [Fact]
    public void Contructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
    {
        int cantidadAtomos = 5;
        int cantidadJugadores = 3;
        IndividuoNuevo individuo = CrearIndividuo(cantidadAtomos, cantidadJugadores);

        int cantidadUnosEsperada = cantidadJugadores - 1;
        int cantidadUnos = individuo.Cromosoma.Count(gen => gen == 1);
        Assert.Equal(cantidadUnosEsperada, cantidadUnos);

        int cantidadCerosEsperada = cantidadAtomos - 1 - cantidadUnosEsperada;
        int cantidadCeros = individuo.Cromosoma.Count(gen => gen == 0);
        Assert.Equal(cantidadCerosEsperada, cantidadCeros);

        Assert.Equal(cantidadCerosEsperada + cantidadUnosEsperada, individuo.Cromosoma.Count);
    }

    [Fact]
    public void Constructor_Cromosoma_UnosAsignaosAleatoriamente()
    {
        int cantidadAtomos = 10;
        int cantidadJugadores = 4;
        var generadorRandom1 = GeneradorNumerosRandomFactory.Crear(1);
        var generadorRandom2 = GeneradorNumerosRandomFactory.Crear(2);
        IndividuoNuevo individuo1 = CrearIndividuo(cantidadAtomos, cantidadJugadores, generadorRandom1);
        IndividuoNuevo individuo2 = CrearIndividuo(cantidadAtomos, cantidadJugadores, generadorRandom2);

        bool sonIguales = individuo1.Cromosoma.SequenceEqual(individuo2.Cromosoma);
        Assert.False(sonIguales, "Se esperaban cromosomas diferentes entre dos instancias.");
    }

    private IndividuoNuevo CrearIndividuo(int cantidadAtomos, int cantidadAgentes, GeneradorNumerosRandom generadorRandom = null)
    {
        var generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
        var individuo = new IndividuoNuevo(cantidadAtomos, cantidadAgentes, generador);
        return individuo;
    }
}
