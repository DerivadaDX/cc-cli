using Common;
using Solver.Individuos;

namespace Solver
{
    public class PoblacionFactory
    {
        private static Poblacion _poblacion;

        public static Poblacion Crear(
            int tamaño,
            InstanciaProblema problema,
            TipoIndividuo tipoIndividuo,
            GeneradorNumerosRandom generadorRandom
        )
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));

            var poblacion = _poblacion ?? new Poblacion(tamaño, generadorRandom);
            for (int i = 0; i < tamaño; i++)
            {
                Individuo individuo = IndividuoFactory.CrearAleatorio(problema, tipoIndividuo);
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
