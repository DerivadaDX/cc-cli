
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
            MostrarMensajeConColor(mensaje, ConsoleColor.White);
        }

        internal void MostrarAdvertencia(string mensaje)
        {
            MostrarMensajeConColor(mensaje, ConsoleColor.Yellow);
        }

        internal void MostrarError(string mensaje)
        {
            MostrarMensajeConColor(mensaje, ConsoleColor.Red);
        }

        internal void MostrarProgreso(string mensaje)
        {
            _consola.ForegroundColor(ConsoleColor.White);
            _consola.Write($"\r{mensaje}");
            _consola.ResetColor();
        }

        private void MostrarMensajeConColor(string mensaje, ConsoleColor color)
        {
            _consola.ForegroundColor(color);
            _consola.WriteLine(mensaje);
            _consola.ResetColor();
        }
    }
}
