namespace Solver.Random
{
    internal class GeneradorNumerosRandom
    {
        private readonly System.Random _random;

        internal GeneradorNumerosRandom()
        {
            _random = new System.Random();
        }

        internal GeneradorNumerosRandom(int seed)
        {
            if (seed < 0)
                throw new ArgumentException($"La semilla no puede ser negativa: {seed}", nameof(seed));

            _random = new System.Random(seed);
        }

        internal virtual int Siguiente()
        {
            return _random.Next();
        }

        internal virtual int Siguiente(int minimo, int maximo)
        {
            return _random.Next(minimo, maximo);
        }
    }
}
