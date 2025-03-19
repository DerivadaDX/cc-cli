namespace Solver.Individuos
{
    internal class IndividuoMutacionSwap
    {
        internal IndividuoMutacionSwap(List<int> cromosoma, InstanciaProblema problema)
        {
            if (cromosoma.Count == 0)
                throw new ArgumentException("El cromosoma no puede estar vacío");

            int cantidadCortesEsperada = problema.Jugadores.Count - 1;
            int cantidadAsigncacionesEsperada = problema.Jugadores.Count;

            int cantidadGenesEsperada = cantidadCortesEsperada + cantidadAsigncacionesEsperada;
            if (cromosoma.Count != cantidadGenesEsperada)
            {
                string mensaje = $"Cantidad de genes inválida. Esperada: {cantidadGenesEsperada}, recibida: {cromosoma.Count}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            List<int> cortes = [.. cromosoma.Take(cantidadCortesEsperada).Order()];
            if (cortes.First() < 0)
            {
                string mensaje = $"Posición del primer corte no puede ser negativa: {cortes.First()}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            if (cortes.Last() > problema.CantidadAtomos)
            {
                string mensaje = $"Posición del último corte no puede superar a {problema.CantidadAtomos}: {cortes.Last()}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }
    }
}
