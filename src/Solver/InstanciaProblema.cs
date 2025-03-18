
namespace Solver
{
    internal class InstanciaProblema
    {
        internal InstanciaProblema()
        {
        }

        internal List<Jugador> Jugadores { get; } = [];
        private HashSet<int> AtomosValorados { get; } = [];

        internal void AgregarJugador(Jugador jugador)
        {
            if (Jugadores.Any(j => j.Id == jugador.Id))
                throw new InvalidOperationException($"Ya existe un jugador con el id {jugador.Id}");

            foreach (Atomo atomo in jugador.Valoraciones)
            {
                if (AtomosValorados.Contains(atomo.Posicion))
                {
                    string mensaje = $"El jugador #{jugador.Id} valora al átomo #{atomo.Posicion} que ya fue valorado por otro";
                    throw new InvalidOperationException(mensaje);
                }
            }

            Jugadores.Add(jugador);
            foreach (Atomo atomo in jugador.Valoraciones)
            {
                AtomosValorados.Add(atomo.Posicion);
            }
        }
    }
}
