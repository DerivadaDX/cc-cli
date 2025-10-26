using Solver.Individuos;

namespace App.Commands.Resolver
{
    internal class ParametrosSolucion
    {
        internal string RutaInstancia { get; set; }
        internal int LimiteGeneraciones { get; set; }
        internal int CantidadIndividuos { get; set; }
        internal int LimiteEstancamiento { get; set; }
        internal TipoIndividuo TipoIndividuos { get; set; }
        internal int Seed { get; set; }
    }
}
