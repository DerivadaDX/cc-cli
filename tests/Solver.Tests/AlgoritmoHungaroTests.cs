public class AlgoritmoHungaroTests
{
    [Fact]
    public void EncontrarAsignacionQueMinimizaEnvidia_ValoracionesSimples_RetornaAsignacionCorrecta()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 50m, 10m },
            { 10m, 50m },
        };

        int[] asignaciones = algoritmoHungaro.EncontrarAsignacionQueMinimizaEnvidia(valoraciones);

        var asignacionesEsperadas = new int[] { 0, 1 };
        Assert.Equal(asignacionesEsperadas, asignaciones);
    }

    [Fact]
    public void EncontrarAsignacionQueMinimizaEnvidia_ValoracionesIguales_RetornaCualquierAsignacion()
    {
        var algoritmoHungaro = new AlgoritmoHungaro();
        decimal[,] valoraciones = new decimal[,]
        {
            { 20m, 20m },
            { 20m, 20m },
        };

        int[] asignaciones = algoritmoHungaro.EncontrarAsignacionQueMinimizaEnvidia(valoraciones);

        var asignacionesEsperadas1 = new int[] { 0, 1 };
        var asignacionesEsperadas2 = new int[] { 1, 0 };

        bool esAsignacionValida =
            asignaciones.SequenceEqual(asignacionesEsperadas1) || asignaciones.SequenceEqual(asignacionesEsperadas2);
        Assert.True(esAsignacionValida, $"La asignación devuelta no es la esperada: [{string.Join(", ", asignaciones)}]");
    }
}
