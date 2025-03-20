namespace Solver.Random
{
    internal class GeneradorNumerosRandomFactory
    {
        private static GeneradorNumerosRandom _generador;

        internal static GeneradorNumerosRandom Crear()
        {
            var generador = _generador ?? new GeneradorNumerosRandom();
            return generador;
        }

        internal static GeneradorNumerosRandom Crear(int seed)
        {
            var generador = _generador ?? new GeneradorNumerosRandom(seed);
            return generador;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        internal static void SetearGenerador(GeneradorNumerosRandom generador)
        {
            _generador = generador;
        }
#endif
    }
}
