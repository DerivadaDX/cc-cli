using Common;

namespace Solver.Individuos
{
    public static class IndividuoFactory
    {
        public static Individuo CrearAleatorio(InstanciaProblema problema, TipoIndividuo tipoIndividuo)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));

            var random = GeneradorNumerosRandomFactory.Crear(0); // TODO: Cambiar
            List<int> cromosoma = GenerarCromosoma(problema, random);

            Individuo individuo = tipoIndividuo switch
            {
                TipoIndividuo.IntercambioAsignaciones => new IndividuoIntercambioAsignaciones(cromosoma, problema),
                TipoIndividuo.OptimizacionAsignaciones => new IndividuoOptimizacionAsignaciones(cromosoma, problema),
                _ => throw new ArgumentException($"Tipo de individuo no soportado: {tipoIndividuo}", nameof(tipoIndividuo)),
            };

            return individuo;
        }

        private static List<int> GenerarCromosoma(InstanciaProblema problema, GeneradorNumerosRandom random)
        {
            List<int> cortes = GenerarCortes(problema, random);
            List<int> asignaciones = GenerarAsignaciones(problema, random);

            var cromosoma = cortes.Concat(asignaciones).ToList<int>();
            return cromosoma;
        }

        private static List<int> GenerarCortes(InstanciaProblema problema, GeneradorNumerosRandom random)
        {
            int cantidadCortes = problema.Agentes.Count - 1;

            var cortes = new List<int>();
            for (int i = 0; i < cantidadCortes; i++)
            {
                int corte = random.Siguiente(problema.CantidadAtomos + 1);
                cortes.Add(corte);
            }

            return cortes;
        }

        private static List<int> GenerarAsignaciones(InstanciaProblema problema, GeneradorNumerosRandom random)
        {
            int cantidadAgentes = problema.Agentes.Count;

            var asignaciones = Enumerable.Range(1, cantidadAgentes).ToList<int>();
            for (int i = asignaciones.Count - 1; i > 0; i--)
            {
                int j = random.Siguiente(i + 1);
                (asignaciones[i], asignaciones[j]) = (asignaciones[j], asignaciones[i]);
            }

            return asignaciones;
        }
    }
}
