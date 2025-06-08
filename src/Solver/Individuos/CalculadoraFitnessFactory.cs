namespace Solver.Individuos
{
    internal class CalculadoraFitnessFactory
    {
        private static CalculadoraFitness _calculadora;

        public static CalculadoraFitness Crear()
        {
            var calculadora = _calculadora ?? new CalculadoraFitness();
            return calculadora;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        public static void SetearCalculadora(CalculadoraFitness calculadora)
        {
            _calculadora = calculadora;
        }
#endif
    }
}
