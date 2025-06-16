
namespace App
{
    internal class Presentador
    {
        private readonly ConsoleProxy _consola;

        internal Presentador(ConsoleProxy consola)
        {
            ArgumentNullException.ThrowIfNull(consola, nameof(consola));
            _consola = consola;
        }

        internal void MostrarInfo(string mensaje)
        {
            _consola.ForegroundColor(ConsoleColor.White);
            _consola.WriteLine(mensaje);
            _consola.ResetColor();
        }

        internal void MostrarProgreso(string mensaje)
        {
            _consola.ForegroundColor(ConsoleColor.White);
            _consola.Write($"\r{mensaje}");
            _consola.ResetColor();
        }

        internal void MostrarAdvertencia(string mensaje)
        {
            _consola.ForegroundColor(ConsoleColor.Yellow);
            _consola.WriteLine(mensaje);
            _consola.ResetColor();
        }
    }
}
