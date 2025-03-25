namespace Common
{
    public class GeneradorNumerosRandom
    {
        private readonly Random _random;

        public GeneradorNumerosRandom()
        {
            _random = new Random();
        }

        public GeneradorNumerosRandom(int seed)
        {
            if (seed < 0)
                throw new ArgumentException($"La semilla no puede ser negativa: {seed}", nameof(seed));

            _random = new Random(seed);
        }

        public virtual int Siguiente()
        {
            return _random.Next();
        }

        public virtual int Siguiente(int minimo, int maximo)
        {
            return _random.Next(minimo, maximo);
        }
    }
}
