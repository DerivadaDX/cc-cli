namespace App
{
    internal class ParametrosSolucion
    {
        internal ParametrosSolucion(string rutaInstancia, int maxGeneraciones)
        {
            RutaInstancia = rutaInstancia;
            MaxGeneraciones = maxGeneraciones;
        }

        internal string RutaInstancia { get; }
        internal int MaxGeneraciones { get; }
    }
}
