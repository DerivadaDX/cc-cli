﻿namespace Solver
{
    internal class Atomo
    {
        internal Atomo(int posicion, decimal valoracion)
        {
            if (posicion <= 0)
                throw new ArgumentException($"La posición debe ser positiva: {posicion}", nameof(posicion));

            if (valoracion < 0)
                throw new ArgumentException($"La valoración no puede ser negativa: {valoracion}", nameof(valoracion));

            if (valoracion > 1)
                throw new ArgumentException($"La valoración no puede ser mayor que 1: {valoracion}", nameof(valoracion));

            Posicion = posicion;
            Valoracion = valoracion;
        }

        internal int Posicion { get; }
        internal decimal Valoracion { get; }
    }
}
