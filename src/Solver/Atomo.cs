namespace Solver
{
    internal class Atomo
    {
        internal Atomo(int posicion, decimal valoracion)
        {
            if (posicion < 1)
            {
                string mensaje = $"La posición debe ser mayor o igual a 1 (valor: {posicion})";
                throw new ArgumentOutOfRangeException(nameof(posicion), mensaje);
            }

            if (valoracion < 0)
            {
                string mensaje = $"La valoración no puede ser negativa (valor: {valoracion})";
                throw new ArgumentOutOfRangeException(nameof(valoracion), mensaje);
            }

            Posicion = posicion;
            Valoracion = valoracion;
        }

        internal int Posicion { get; }
        internal decimal Valoracion { get; }
    }
}
