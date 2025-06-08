using Solver.Individuos;

namespace Solver
{
    internal class PoblacionFactory
    {
        private static Poblacion _poblacion;

        public static Poblacion CrearInicial(int tamaño, IIndividuoFactory individuoFactory)
        {
            ArgumentNullException.ThrowIfNull(individuoFactory, nameof(individuoFactory));

            var poblacion = _poblacion ?? new Poblacion(tamaño);

            for (int i = 0; i < tamaño; i++)
            {
                Individuo individuo = individuoFactory.CrearAleatorio();
                poblacion.Individuos.Add(individuo);
            }

            return poblacion;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        public static void SetearPoblacion(Poblacion poblacion)
        {
            _poblacion = poblacion;
        }
#endif
    }
}
