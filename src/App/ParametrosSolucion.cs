namespace App
{
    internal class ParametrosSolucion
    {
        internal ParametrosSolucion(string rutaInstancia, int limiteGeneraciones, int cantidadIndividuos)
        {
            RutaInstancia = rutaInstancia;
            LimiteGeneraciones = limiteGeneraciones;
            CantidadIndividuos = cantidadIndividuos;
        }

        internal string RutaInstancia { get; }
        internal int LimiteGeneraciones { get; }
        internal int CantidadIndividuos { get; }
    }
}
