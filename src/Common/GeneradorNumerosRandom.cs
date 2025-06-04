namespace Common
{
    public class GeneradorNumerosRandom
    {
        private readonly Random _random;

        internal GeneradorNumerosRandom()
        {
            _random = new Random();
        }

        internal GeneradorNumerosRandom(int seed)
        {
            if (seed < 0)
                throw new ArgumentOutOfRangeException(nameof(seed), $"La semilla no puede ser negativa (valor: {seed})");

            _random = new Random(seed);
        }

        public virtual int Siguiente()
        {
            return _random.Next();
        }

        public virtual int Siguiente(int maximo)
        {
            return _random.Next(maximo);
        }

        public virtual int Siguiente(int minimo, int maximo)
        {
            return _random.Next(minimo, maximo);
        }
    }
}
