namespace App.Commands.Resolver
{
    internal class ParametrosSolucion
    {
        internal ParametrosSolucion(string rutaInstancia, int limiteGeneraciones, int cantidadIndividuos, int limiteEstancamiento)
        {
            RutaInstancia = rutaInstancia;
            LimiteGeneraciones = limiteGeneraciones;
            CantidadIndividuos = cantidadIndividuos;
            LimiteEstancamiento = limiteEstancamiento;
        }

        internal string RutaInstancia { get; }
        internal int LimiteGeneraciones { get; }
        internal int CantidadIndividuos { get; }
        internal int LimiteEstancamiento { get; }
    }
}
