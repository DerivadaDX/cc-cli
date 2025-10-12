using System.Globalization;
using Common;

namespace Solver.Individuos
{
    public abstract class Individuo
    {
        protected readonly InstanciaProblema _problema;
        protected readonly GeneradorNumerosRandom _random;
        protected readonly CalculadoraFitness _calculadoraFitness;

        protected Individuo(List<int> cromosoma, InstanciaProblema problema)
        {
            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ValidarCromosoma(cromosoma, problema);

            Cromosoma = cromosoma;
            _problema = problema;
            _random = GeneradorNumerosRandomFactory.Crear(0); // TODO: Cambiar
            _calculadoraFitness = CalculadoraFitnessFactory.Crear();
        }

        internal virtual void Mutar()
        {
            MutarCortes();
            MutarAsignaciones();
        }

        internal virtual Individuo Cruzar(Individuo otro)
        {
            if (Cromosoma.Count != otro.Cromosoma.Count)
                throw new ArgumentException("Los padres deben tener la misma cantidad de cromosomas para poder cruzarlos.");

            List<int> cortesHijo = CruzaCortes(otro);
            List<int> asignacionesHijo = CruzaAsignaciones(otro);

            List<int> cromosomaHijo = [];
            cromosomaHijo.AddRange(cortesHijo);
            cromosomaHijo.AddRange(asignacionesHijo);

            Individuo hijo = CrearNuevoIndividuo(cromosomaHijo);
            return hijo;
        }

        protected abstract void MutarAsignaciones();
        protected abstract Individuo CrearNuevoIndividuo(List<int> cromosoma);

        internal virtual List<int> Cromosoma { get; }

        public override string ToString()
        {
            List<int> cromosomaOrdenado = OrdenarCortes(Cromosoma, _problema);
            string cromosoma = string.Join(", ", cromosomaOrdenado);
            decimal fitness = Fitness();

            string resultado = $"Cromosoma=[{cromosoma}], Fitness={fitness.ToString(CultureInfo.InvariantCulture)}";
            return resultado;
        }

        internal virtual decimal Fitness()
        {
            decimal fitness = _calculadoraFitness.CalcularFitness(this, _problema);
            return fitness;
        }

        private void ValidarCromosoma(List<int> cromosoma, InstanciaProblema problema)
        {
            if (cromosoma.Count == 0)
                throw new ArgumentException("El cromosoma no puede estar vacío", nameof(cromosoma));

            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortesEsperada = cantidadAgentes - 1;

            ValidarCantidadGenes(cromosoma, cantidadAgentes);
            ValidarCortes(cromosoma, problema, cantidadCortesEsperada);
            ValidarAsignaciones(cromosoma, cantidadAgentes, cantidadCortesEsperada);
        }

        private void ValidarCantidadGenes(List<int> cromosoma, int cantidadAgentes)
        {
            int cantidadCortesEsperada = cantidadAgentes - 1;
            int cantidadGenesEsperada = cantidadCortesEsperada + cantidadAgentes;

            if (cromosoma.Count != cantidadGenesEsperada)
            {
                string mensaje = $"Cantidad de genes inválida. Esperada: {cantidadGenesEsperada}, recibida: {cromosoma.Count}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void ValidarCortes(List<int> cromosoma, InstanciaProblema problema, int cantidadCortesEsperada)
        {
            List<int> cortes = [.. cromosoma.Take(cantidadCortesEsperada).Order()];

            if (cantidadCortesEsperada == 0 || cortes.Count == 0)
                return;

            if (cortes.First() < 0)
            {
                string mensaje = $"El primer corte no puede ser negativo: {cortes.First()}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            if (cortes.Last() > problema.CantidadAtomos)
            {
                string mensajeTemplate = "El último corte no puede superar la cantidad de átomos ({0}): {1}";
                string mensaje = string.Format(mensajeTemplate, problema.CantidadAtomos, cortes.Last());
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void ValidarAsignaciones(List<int> cromosoma, int cantidadAgentes, int cantidadCortesEsperada)
        {
            List<int> asignaciones = [.. cromosoma.Skip(cantidadCortesEsperada)];
            List<int> fueraDeRango = [.. asignaciones.Where(a => a < 1 || a > cantidadAgentes).Distinct().Order()];
            if (fueraDeRango.Count > 0)
            {
                string mensaje = $"Hay asignaciones fuera de rango [1, {cantidadAgentes}]: ({string.Join(", ", fueraDeRango)})";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            List<int> repetidas = [.. asignaciones.GroupBy(a => a).Where(g => g.Count() > 1).Select(g => g.Key)];
            if (repetidas.Count > 0)
            {
                string mensaje = $"Hay asignaciones repetidas a agentes: ({string.Join(", ", repetidas)})";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void MutarCortes()
        {
            int cantidadCortes = _problema.Agentes.Count - 1;
            int rango = _problema.CantidadAtomos + 1;
            int L = Cromosoma.Count;

            for (int i = 0; i < cantidadCortes; i++)
            {
                double probabilidadMutacion = _random.SiguienteDouble();
                if (probabilidadMutacion < 1.0 / L)
                {
                    int direccion = _random.Siguiente(2) == 0 ? -1 : 1;
                    int nuevoValor = (Cromosoma[i] + direccion + rango) % rango;
                    Cromosoma[i] = nuevoValor;
                }
            }
        }

        private List<int> CruzaCortes(Individuo otro)
        {
            int cantidadCortes = _problema.Agentes.Count - 1;
            var cortesPadre1 = Cromosoma.Take(cantidadCortes).ToList<int>();
            var cortesPadre2 = otro.Cromosoma.Take(cantidadCortes).ToList<int>();

            if (cantidadCortes == 0)
                return [];

            int indiceCorte = cantidadCortes > 1 ? _random.Siguiente(1, cantidadCortes) : cantidadCortes;
            var cortesHijo = cortesPadre1.Take(indiceCorte).ToList<int>();
            cortesHijo.AddRange(cortesPadre2.Skip(indiceCorte));
            return cortesHijo;
        }

        private List<int> CruzaAsignaciones(Individuo otro)
        {
            int cantidadCortes = _problema.Agentes.Count - 1;
            int cantidadAsignaciones = _problema.Agentes.Count;

            var asignacionesPadre1 = Cromosoma.Skip(cantidadCortes).ToList<int>();
            var asignacionesPadre2 = otro.Cromosoma.Skip(cantidadCortes).ToList<int>();

            if (cantidadAsignaciones <= 1)
                return [.. asignacionesPadre1];

            var asignacionesHijo = Enumerable.Repeat(-1, cantidadAsignaciones).ToList<int>();
            int indiceInicioSegmento = _random.Siguiente(cantidadAsignaciones);
            int indiceFinSegmento = _random.Siguiente(indiceInicioSegmento, cantidadAsignaciones);

            for (int i = indiceInicioSegmento; i <= indiceFinSegmento; i++)
                asignacionesHijo[i] = asignacionesPadre1[i];

            int indicePadre2 = 0;
            int indiceHijo = (indiceFinSegmento + 1) % cantidadAsignaciones;
            while (asignacionesHijo.Contains(-1))
            {
                int candidato = asignacionesPadre2[indicePadre2++];
                if (!asignacionesHijo.Contains(candidato))
                {
                    asignacionesHijo[indiceHijo] = candidato;
                    indiceHijo = (indiceHijo + 1) % cantidadAsignaciones;
                }
            }

            return asignacionesHijo;
        }

        private List<int> OrdenarCortes(List<int> cromosoma, InstanciaProblema problema)
        {
            int cantidadCortes = problema.Agentes.Count - 1;
            if (cantidadCortes == 0)
                return cromosoma;

            var cortesOrdenados = cromosoma.Take(cantidadCortes).OrderBy(x => x).ToList<int>();
            var asignaciones = cromosoma.Skip(cantidadCortes).ToList<int>();
            var cromosomaOrdenado = cortesOrdenados.Concat(asignaciones).ToList<int>();

            return cromosomaOrdenado;
        }
    }
}
