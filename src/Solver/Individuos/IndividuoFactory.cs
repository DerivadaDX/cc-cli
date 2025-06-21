using Common;

namespace Solver.Individuos
{
    public static class IndividuoFactory
    {
        public static Individuo CrearAleatorio(InstanciaProblema problema, TipoIndividuo tipoIndividuo)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));

            var random = GeneradorNumerosRandomFactory.Crear();
            int cantidadAgentes = problema.Agentes.Count;
            List<int> cortes = GenerarCortes(random, problema, cantidadAgentes - 1);
            List<int> asignaciones = GenerarAsignaciones(random, cantidadAgentes);
            var cromosoma = cortes.Concat(asignaciones).ToList<int>();

            Individuo individuo = tipoIndividuo switch
            {
                TipoIndividuo.Intercambio => new IndividuoIntercambioAsignaciones(cromosoma, problema),
                TipoIndividuo.Optimizacion => new IndividuoOptimizacionAsignaciones(cromosoma, problema),
                _ => throw new ArgumentException($"Tipo de individuo no soportado: {tipoIndividuo}", nameof(tipoIndividuo))
            };

            return individuo;
        }

        private static List<int> GenerarCortes(GeneradorNumerosRandom random, InstanciaProblema problema, int cantidadCortes)
        {
            var cortes = new List<int>();
            for (int i = 0; i < cantidadCortes; i++)
            {
                int corte = random.Siguiente(problema.CantidadAtomos + 1);
                cortes.Add(corte);
            }
            return cortes;
        }

        private static List<int> GenerarAsignaciones(GeneradorNumerosRandom random, int cantidadAgentes)
        {
            var asignaciones = Enumerable.Range(1, cantidadAgentes).ToList();
            // Fisher–Yates shuffle
            for (int i = asignaciones.Count - 1; i > 0; i--)
            {
                int j = random.Siguiente(i + 1);
                (asignaciones[i], asignaciones[j]) = (asignaciones[j], asignaciones[i]);
            }
            return asignaciones;
        }
    }
}
