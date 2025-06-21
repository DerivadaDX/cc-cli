using Solver.Individuos;

namespace App.Commands.Resolver
{
    internal class ParametrosSolucion
    {
        internal ParametrosSolucion(string rutaInstancia, int limiteGeneraciones, int cantidadIndividuos, int limiteEstancamiento, TipoIndividuo tipoIndividuo)
        {
            RutaInstancia = rutaInstancia;
            LimiteGeneraciones = limiteGeneraciones;
            CantidadIndividuos = cantidadIndividuos;
            LimiteEstancamiento = limiteEstancamiento;
            TipoIndividuo = tipoIndividuo;
        }

        internal string RutaInstancia { get; }
        internal int LimiteGeneraciones { get; }
        internal int CantidadIndividuos { get; }
        internal int LimiteEstancamiento { get; }
        internal TipoIndividuo TipoIndividuo { get; }
    }
}
