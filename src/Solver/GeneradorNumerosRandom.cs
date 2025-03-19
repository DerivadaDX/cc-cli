namespace Solver
{
    internal class GeneradorNumerosRandom : IGeneradorNumerosRandom
    {
        private readonly Random _random;

        internal GeneradorNumerosRandom()
        {
            _random = new Random();
        }

        internal GeneradorNumerosRandom(int seed)
        {
            if (seed < 0)
                throw new ArgumentException($"La semilla no puede ser negativa: {seed}", nameof(seed));

            _random = new Random(seed);
        }

        public int Siguiente()
        {
            return _random.Next();
        }

        public int Siguiente(int minimo, int maximo)
        {
            return _random.Next(minimo, maximo);
        }
    }
}
