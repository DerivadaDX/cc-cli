﻿
namespace Solver
{
    internal class Jugador
    {
        internal Jugador(int id)
        {
            Id = id;
        }

        internal int Id { get; }
        internal List<Atomo> Valoraciones { get; } = [];

        internal void AgregarValoracion(Atomo atomo)
        {
            if (Valoraciones.Any(v => v.Posicion == atomo.Posicion))
                throw new InvalidOperationException($"Ya existe una valoración para el átomo #{atomo.Posicion}");

            Valoraciones.Add(atomo);
        }
    }
}