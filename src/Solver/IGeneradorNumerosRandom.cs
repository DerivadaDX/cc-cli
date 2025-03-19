namespace Solver
{
    internal interface IGeneradorNumerosRandom
    {
        int Siguiente();
        int Siguiente(int minimo, int maximo);
    }
}
