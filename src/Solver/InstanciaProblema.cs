
namespace Solver
{
    internal class InstanciaProblema
    {
        internal InstanciaProblema()
        {
        }

        internal List<Jugador> Jugadores { get; } = [];
        internal int CantidadAtomos => AtomosValorados.Count;
        private HashSet<int> AtomosValorados { get; } = [];

        internal static InstanciaProblema CrearDesdeMatrizDeValoraciones(decimal[][] matrizValoraciones)
        {
            var instanciaProblema = new InstanciaProblema();

            for (int indiceJugador = 0; indiceJugador < matrizValoraciones.Length; indiceJugador++)
            {
                var jugador = new Jugador(indiceJugador + 1);
                for (int indiceAtomo = 0; indiceAtomo < matrizValoraciones[indiceJugador].Length; indiceAtomo++)
                {
                    decimal valoracion = matrizValoraciones[indiceJugador][indiceAtomo];
                    if (valoracion > 0)
                    {
                        var atomo = new Atomo(indiceAtomo + 1, valoracion);
                        jugador.AgregarValoracion(atomo);
                    }
                }

                instanciaProblema.AgregarJugador(jugador);
            }

            return instanciaProblema;
        }

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
