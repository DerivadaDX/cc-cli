namespace App
{
    internal class Presentador
    {
        private readonly ConsoleProxy _consola;
        private bool _lineaEnCurso = false;

        internal Presentador(ConsoleProxy consola)
        {
            ArgumentNullException.ThrowIfNull(consola, nameof(consola));
            _consola = consola;
        }

        internal void MostrarInfo(string mensaje)
        {
            AgregarSaltoSiNecesario();
            MostrarMensajeConColor(mensaje, ConsoleColor.White);
            _lineaEnCurso = false;
        }

        internal void MostrarExito(string mensaje)
        {
            AgregarSaltoSiNecesario();
            MostrarMensajeConColor(mensaje, ConsoleColor.Green);
            _lineaEnCurso = false;
        }

        internal void MostrarAdvertencia(string mensaje)
        {
            AgregarSaltoSiNecesario();
            MostrarMensajeConColor(mensaje, ConsoleColor.Yellow);
            _lineaEnCurso = false;
        }

        internal void MostrarError(string mensaje)
        {
            AgregarSaltoSiNecesario();
            MostrarMensajeConColor(mensaje, ConsoleColor.Red);
            _lineaEnCurso = false;
        }

        internal void MostrarProgreso(string mensaje)
        {
            _consola.ForegroundColor(ConsoleColor.White);
            _consola.Write($"\r{mensaje}");
            _consola.ResetColor();
            _lineaEnCurso = true;
        }

        private void AgregarSaltoSiNecesario()
        {
            if (_lineaEnCurso)
                _consola.WriteLine(string.Empty);
        }

        private void MostrarMensajeConColor(string mensaje, ConsoleColor color)
        {
            _consola.ForegroundColor(color);
            _consola.WriteLine(mensaje);
            _consola.ResetColor();
        }
    }
}
