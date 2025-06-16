namespace App
{
    internal class ConsoleProxyFactory
    {
        private static ConsoleProxy _consola;

        internal static ConsoleProxy Crear()
        {
            var consola = _consola ?? new ConsoleProxy();
            return consola;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        internal static void SetearConsola(ConsoleProxy consola)
        {
            _consola = consola;
        }
#endif
    }
}
