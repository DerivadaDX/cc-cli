using Common;
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
    public void Constructor_CantidadAgentesMayorQueCantidadAtomos_LanzaArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CrearIndividuo(cantidadAtomos: 5, cantidadAgentes: 6));
        Assert.Contains("no puede ser mayor que la cantidad de átomos", ex.Message);
        Assert.Equal("cantidadAgentes", ex.ParamName);
    }

    [Fact]
    public void Constructor_GeneradorNumerosRandomNull_LanzaArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoNuevo(cantidadAtomos: 5, cantidadAgentes: 3, null));
        Assert.Equal("generadorRandom", ex.ParamName);
    }

    [Fact]
    public void Contructor_Cromosoma_CantidadCorrectaDeCerosYUnos()
    {
        int cantidadAtomos = 5;
        int cantidadAgentes = 3;
        IndividuoNuevo individuo = CrearIndividuo(cantidadAtomos, cantidadAgentes);

        int longitudCromosomaEsperada = cantidadAtomos - 1;
        Assert.Equal(longitudCromosomaEsperada, individuo.Cromosoma.Count);

        int cantidadUnosEsperada = cantidadAgentes - 1;
        int cantidadUnos = individuo.Cromosoma.Count(gen => gen == 1);
        Assert.Equal(cantidadUnosEsperada, cantidadUnos);

        int cantidadCerosEsperada = longitudCromosomaEsperada - cantidadUnosEsperada;
        int cantidadCeros = individuo.Cromosoma.Count(gen => gen == 0);
        Assert.Equal(cantidadCerosEsperada, cantidadCeros);
    }

    [Fact]
    public void Constructor_Cromosoma_UnosAsignadosAleatoriamente()
    {
        int cantidadAtomos = 10;
        int cantidadAgentes = 4;
        var generadorRandom1 = GeneradorNumerosRandomFactory.Crear(1);
        var generadorRandom2 = GeneradorNumerosRandomFactory.Crear(2);
        IndividuoNuevo individuo1 = CrearIndividuo(cantidadAtomos, cantidadAgentes, generadorRandom1);
        IndividuoNuevo individuo2 = CrearIndividuo(cantidadAtomos, cantidadAgentes, generadorRandom2);

        bool sonIguales = individuo1.Cromosoma.SequenceEqual(individuo2.Cromosoma);
        Assert.False(sonIguales, "Se esperaban cromosomas diferentes entre dos instancias.");
    }

    private IndividuoNuevo CrearIndividuo(int cantidadAtomos, int cantidadAgentes, GeneradorNumerosRandom generadorRandom = null)
    {
        var generador = generadorRandom ?? GeneradorNumerosRandomFactory.Crear(1);
        var individuo = new IndividuoNuevo(cantidadAtomos, cantidadAgentes, generador);
        return individuo;
    }

    /*
     * Tests faltantes:
     * - Inicializar el cromosoma calcula la asignación óptima de porciones
     * - Mutar mantiene la cantidad correcta de unos y ceros
     * - Mutar cambia la posición de algunos unos y ceros si cumple la probabilidad
     * - Mutar produce diferentes resultados en distintas ejecuciones
     * - Mutar actualiza la asignación óptima de porciones
     * - Cruzar produce un hijo con la cantidad correcta de unos y ceros
     * - Cruzar produce un hijo con genes tomados de ambos padres
     */
}
