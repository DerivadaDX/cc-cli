namespace Common
{
    public class GeneradorNumerosRandomFactory
    {
        private static GeneradorNumerosRandom _generador;

        public static GeneradorNumerosRandom Crear(int seed)
        {
            var generador = _generador ?? new GeneradorNumerosRandom(seed);
            return generador;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        public static void SetearGenerador(GeneradorNumerosRandom generador)
        {
            _generador = generador;
        }
#endif
    }
}
