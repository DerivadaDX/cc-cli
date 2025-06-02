namespace App
{
    internal class ParametrosSolucion
    {
        internal ParametrosSolucion(string rutaInstancia, int maxGeneraciones, int tamañoPoblacion)
        {
            RutaInstancia = rutaInstancia;
            MaxGeneraciones = maxGeneraciones;
            TamañoPoblacion = tamañoPoblacion;
        }

        internal string RutaInstancia { get; }
        internal int MaxGeneraciones { get; }
        internal int TamañoPoblacion { get; }
    }
}
