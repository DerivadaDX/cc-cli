namespace Solver
{
    internal class AlgoritmoGenetico
    {
        private readonly int _tamañoPoblacion;

        public AlgoritmoGenetico(int tamañoPoblacion)
        {
            if (tamañoPoblacion <= 0)
                throw new ArgumentException($"El tamaño de la población debe ser positivo: {tamañoPoblacion}");

            _tamañoPoblacion = tamañoPoblacion;
        }
    }
}
