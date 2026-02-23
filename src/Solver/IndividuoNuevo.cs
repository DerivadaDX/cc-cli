using Common;
using System.Globalization;

namespace Solver
{
    internal class IndividuoNuevo
    {
        private readonly InstanciaProblema _problema;
        private readonly GeneradorNumerosRandom _generadorRandom;
        private readonly CalculadoraValoracionesPorciones _calculadoraValoraciones;
        private readonly AlgoritmoHungaro _algoritmoHungaro;
        private readonly List<int> _cromosoma;
        private List<int> _asignaciones = [];
        private List<int> _preferenciasPorcion = [];
        private List<int> _posicionesCortes = [];
        private decimal _fitness;

        internal IndividuoNuevo(InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            _problema = problema;

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));
            _generadorRandom = generadorRandom;

            int tamañoCromosoma = problema.CantidadAtomos - 1;
            int cantidadUnos = problema.Agentes.Count - 1;
            _cromosoma = GenerarCromosomaAleatorio(tamañoCromosoma, cantidadUnos);

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();

            CalcularEstado();
        }

        private IndividuoNuevo(InstanciaProblema problema, GeneradorNumerosRandom generadorRandom, List<int> cromosoma)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            _problema = problema;

            ArgumentNullException.ThrowIfNull(generadorRandom, nameof(generadorRandom));
            _generadorRandom = generadorRandom;

            ArgumentNullException.ThrowIfNull(cromosoma, nameof(cromosoma));
            _cromosoma = cromosoma;

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();

            CalcularEstado();
        }

        internal IReadOnlyList<int> Cromosoma => _cromosoma;
        internal IReadOnlyList<int> Asignaciones => _asignaciones;
        internal decimal Fitness => _fitness;

        public override string ToString()
        {
            string cortes = string.Join(", ", _cromosoma);

            List<int> asignacionesBaseUno = [.. _asignaciones.Select(asignacion => asignacion + 1)];
            string asignaciones = string.Join(", ", asignacionesBaseUno);

            // TODO: Definir si el fitness debe mostrarse con padding fijo de decimales.
            string fitness = Fitness.ToString(CultureInfo.InvariantCulture);
            string resultado = $"Cortes=[{cortes}], Asignaciones=[{asignaciones}], Fitness={fitness}";
            return resultado;
        }

        internal IndividuoNuevo Cruzar(IndividuoNuevo otro)
        {
            ArgumentNullException.ThrowIfNull(otro, nameof(otro));

            if (_cromosoma.Count != otro._cromosoma.Count)
                throw new ArgumentException("Los cromosomas no tienen la misma longitud.");

            int cantidadCortes = _cromosoma.Count(gen => gen == 1);
            int cantidadCortesOtro = otro._cromosoma.Count(gen => gen == 1);
            if (cantidadCortes != cantidadCortesOtro)
                throw new ArgumentException("Los cromosomas no tienen la misma cantidad de cortes.");

            int cantidadCortesSeleccionados = 0;
            var cortesDisponibles = new List<int>();
            var cromosomaHijo = Enumerable.Repeat(0, _cromosoma.Count).ToList<int>();

            for (int indice = 0; indice < _cromosoma.Count; indice++)
            {
                bool coinciden = _cromosoma[indice] == otro._cromosoma[indice];
                if (coinciden)
                {
                    cromosomaHijo[indice] = _cromosoma[indice];
                    if (_cromosoma[indice] == 1)
                        cantidadCortesSeleccionados++;
                    continue;
                }

                cortesDisponibles.Add(indice);
            }

            while (cantidadCortesSeleccionados < cantidadCortes)
            {
                int indiceRandom = _generadorRandom.Siguiente(cortesDisponibles.Count);
                int indiceSeleccionado = cortesDisponibles[indiceRandom];
                cortesDisponibles.RemoveAt(indiceRandom);

                cromosomaHijo[indiceSeleccionado] = 1;
                cantidadCortesSeleccionados++;
            }

            bool hijoIgualPadre = cromosomaHijo.SequenceEqual(_cromosoma);
            bool hijoIgualOtro = cromosomaHijo.SequenceEqual(otro._cromosoma);
            if (hijoIgualPadre || hijoIgualOtro)
            {
                var indicesConUno = new List<int>();
                var indicesConCero = new List<int>();
                for (int indice = 0; indice < cromosomaHijo.Count; indice++)
                {
                    if (cromosomaHijo[indice] == 1)
                        indicesConUno.Add(indice);
                    else
                        indicesConCero.Add(indice);
                }

                int indiceUno = _generadorRandom.Siguiente(indicesConUno.Count);
                int posicionUno = indicesConUno[indiceUno];
                cromosomaHijo[posicionUno] = 0;

                int indiceCero = _generadorRandom.Siguiente(indicesConCero.Count);
                int posicionCero = indicesConCero[indiceCero];
                cromosomaHijo[posicionCero] = 1;
            }

            // TODO: Pasar una instancia distinta de GeneradorNumerosRandom a los hijos.
            var hijo = new IndividuoNuevo(_problema, _generadorRandom, cromosomaHijo);
            return hijo;
        }

        internal void Mutar()
        {
            List<int> porcionesOrdenadas = ObtenerPorcionesOrdenadasPorPreferencia();
            foreach (int indicePorcion in porcionesOrdenadas)
            {
                bool seAchico = AchicarPorcion(indicePorcion);
                if (!seAchico)
                    continue;

                CalcularEstado();
                return;
            }
        }

        private List<int> GenerarCromosomaAleatorio(int tamaño, int cantidadUnos)
        {
            var cromosoma = Enumerable.Repeat(0, tamaño).ToList<int>();
            var indicesDisponibles = Enumerable.Range(0, tamaño).ToList<int>();
            for (int i = 0; i < cantidadUnos; i++)
            {
                int indiceRandom = _generadorRandom.Siguiente(indicesDisponibles.Count);
                int indiceSeleccionado = indicesDisponibles[indiceRandom];

                cromosoma[indiceSeleccionado] = 1;
                indicesDisponibles.RemoveAt(indiceRandom);
            }

            return cromosoma;
        }

        private void CalcularEstado()
        {
            _posicionesCortes = [];
            for (int indice = 0; indice < _cromosoma.Count; indice++)
            {
                bool esCorte = _cromosoma[indice] == 1;
                if (esCorte)
                    _posicionesCortes.Add(indice + 1);
            }

            decimal[,] valoraciones = _calculadoraValoraciones.CalcularMatrizValoracionesPorcionAgente(_problema, _posicionesCortes);
            _asignaciones = _algoritmoHungaro.CalcularAsignacionOptimaDePorciones(valoraciones);
            _preferenciasPorcion = _calculadoraValoraciones.CalcularPreferenciasPorcion(valoraciones);
            _fitness = CalcularFitness(valoraciones);
        }

        private decimal CalcularFitness(decimal[,] valoracionesPorcionAgente)
        {
            decimal envidiaTotal = 0;

            int cantidadAgentes = _problema.Agentes.Count;
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                int indicePorcionPropia = _asignaciones.FindIndex(asignacion => asignacion == indiceAgente);
                decimal valoracionPropia = valoracionesPorcionAgente[indicePorcionPropia, indiceAgente];
                decimal maxViolacion = 0;

                for (int indicePorcion = 0; indicePorcion < cantidadAgentes; indicePorcion++)
                {
                    if (indicePorcion == indicePorcionPropia)
                        continue;

                    decimal valoracionAjena = valoracionesPorcionAgente[indicePorcion, indiceAgente];
                    if (valoracionAjena <= valoracionPropia)
                        continue;

                    decimal violacion = valoracionAjena - valoracionPropia;
                    if (violacion > maxViolacion)
                        maxViolacion = violacion;
                }

                envidiaTotal += maxViolacion;
            }

            return envidiaTotal;
        }

        private List<int> ObtenerPorcionesOrdenadasPorPreferencia()
        {
            var indices = Enumerable.Range(0, _preferenciasPorcion.Count).ToList<int>();
            var gruposPorPreferencia = indices
                .GroupBy(indice => _preferenciasPorcion[indice])
                .OrderByDescending(grupo => grupo.Key);

            var resultado = new List<int>(indices.Count);
            foreach (IGrouping<int, int> grupo in gruposPorPreferencia)
            {
                var porcionesEmpatadas = grupo.ToList<int>();
                MezclarPorcionesEmpatadas(porcionesEmpatadas);
                resultado.AddRange(porcionesEmpatadas);
            }

            return resultado;
        }

        private void MezclarPorcionesEmpatadas(List<int> porciones)
        {
            for (int indice = porciones.Count - 1; indice > 0; indice--)
            {
                int indiceRandom = _generadorRandom.Siguiente(indice + 1);
                (porciones[indiceRandom], porciones[indice]) = (porciones[indice], porciones[indiceRandom]);
            }
        }

        private bool AchicarPorcion(int indicePorcion)
        {
            int tamañoPorcion = CalcularTamañoPorcion(indicePorcion);
            if (tamañoPorcion <= 1)
                return false;

            bool esPrimeraPorcion = indicePorcion == 0;
            if (esPrimeraPorcion)
            {
                int corteDerecho = _posicionesCortes[0];
                bool pudoMover = MoverCorte(corteDerecho, corteDerecho - 1);
                return pudoMover;
            }

            bool esUltimaPorcion = indicePorcion == _posicionesCortes.Count;
            if (esUltimaPorcion)
            {
                int corteIzquierdo = _posicionesCortes[^1];
                bool pudoMover = MoverCorte(corteIzquierdo, corteIzquierdo + 1);
                return pudoMover;
            }

            int preferenciaIzquierda = _preferenciasPorcion[indicePorcion - 1];
            int preferenciaDerecha = _preferenciasPorcion[indicePorcion + 1];
            bool porcionIzquierdaEsMenosDeseada = preferenciaIzquierda <= preferenciaDerecha;
            if (porcionIzquierdaEsMenosDeseada)
            {
                int corteIzquierdo = _posicionesCortes[indicePorcion - 1];
                bool pudoMoverIzquierdo = MoverCorte(corteIzquierdo, corteIzquierdo + 1);
                return pudoMoverIzquierdo;
            }

            int corteDerechoIntermedio = _posicionesCortes[indicePorcion];
            bool pudoMoverDerecho = MoverCorte(corteDerechoIntermedio, corteDerechoIntermedio - 1);
            return pudoMoverDerecho;
        }

        private int CalcularTamañoPorcion(int indicePorcion)
        {
            int inicio = indicePorcion > 0
                ? _posicionesCortes[indicePorcion - 1] + 1
                : 1;

            int fin = indicePorcion < _posicionesCortes.Count
                ? _posicionesCortes[indicePorcion]
                : _problema.CantidadAtomos;

            int tamaño = fin - inicio + 1;
            return tamaño;
        }

        private bool MoverCorte(int posicionActual, int nuevaPosicion)
        {
            bool nuevaPosicionInvalida = nuevaPosicion < 1 || nuevaPosicion > _cromosoma.Count;
            if (nuevaPosicionInvalida)
                return false;

            int indiceActual = posicionActual - 1;
            bool posicionActualNoEsCorte = _cromosoma[indiceActual] == 0;
            if (posicionActualNoEsCorte)
                return false;

            int indiceNuevo = nuevaPosicion - 1;
            bool posicionNuevaYaEsCorte = _cromosoma[indiceNuevo] == 1;
            if (posicionNuevaYaEsCorte)
                return false;

            _cromosoma[indiceActual] = 0;
            _cromosoma[indiceNuevo] = 1;
            _posicionesCortes.Remove(posicionActual);
            _posicionesCortes.Add(nuevaPosicion);
            _posicionesCortes.Sort();
            return true;
        }
    }
}
