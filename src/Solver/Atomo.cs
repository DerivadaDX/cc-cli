namespace Solver
{
    internal class Atomo
    {
        public Atomo(int posicion)
        {
            if (posicion <= 0)
                throw new ArgumentException("La posición debe ser positiva", nameof(posicion));
        }
    }
}
