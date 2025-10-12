namespace Common
{
    public class GeneradorNumerosRandom
    {
        private static int? _seed;

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

#if DEBUG
        public static void SetearSeed(int seed)
        {
            if (seed < 0)
                throw new ArgumentOutOfRangeException(nameof(seed), $"La semilla no puede ser negativa (valor: {seed})");
            _seed = seed;
        }
#endif

        public static int GenerarSeed()
        {
            if (_seed.HasValue)
                return _seed.Value;

            int seed = Environment.TickCount;
            return seed;
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

        public virtual double SiguienteDouble()
        {
            return _random.NextDouble();
        }
    }
}
