using System.Globalization;
using Common;

namespace Solver.Individuos
{
    internal class IndividuoCortesBinarios : Individuo
    {
        private readonly CalculadoraValoracionesPorciones _calculadoraValoraciones;
        private readonly AlgoritmoHungaro _algoritmoHungaro;
        private List<int> _asignaciones = [];
        private List<int> _preferenciasPorcion = [];
        private List<int> _posicionesCortes = [];
        private decimal _fitness;

        internal IndividuoCortesBinarios(
            List<int> cromosoma, InstanciaProblema problema, GeneradorNumerosRandom generadorRandom)
            : base(cromosoma, problema, generadorRandom)
        {
            ValidarCromosoma(cromosoma, problema);

            _calculadoraValoraciones = CalculadoraValoracionesPorcionesFactory.Crear();
            _algoritmoHungaro = AlgoritmoHungaroFactory.Crear();

            CalcularEstado();
        }

        protected override string FamiliaCromosoma => "cortes-binarios";

        internal IReadOnlyList<int> Asignaciones => _asignaciones;

        public override string ToString()
        {
            string cortes = string.Join(", ", Cromosoma);

            List<int> asignacionesBaseUno = [.. _asignaciones.Select(asignacion => asignacion + 1)];
            string asignaciones = string.Join(", ", asignacionesBaseUno);

            string fitness = _fitness.ToString("0.00", CultureInfo.InvariantCulture);
            string resultado = $"Cortes=[{cortes}], Asignaciones=[{asignaciones}], Fitness={fitness}";
            return resultado;
        }

        internal override Individuo Cruzar(Individuo otro)
        {
            ValidarCompatibilidadCruce(otro);
            IndividuoCortesBinarios otroCortesBinarios = (IndividuoCortesBinarios)otro;
            IndividuoCortesBinarios hijo = Cruzar(otroCortesBinarios);
            return hijo;
        }

        internal IndividuoCortesBinarios Cruzar(IndividuoCortesBinarios otro)
        {
            ArgumentNullException.ThrowIfNull(otro, nameof(otro));

            if (Cromosoma.Count != otro.Cromosoma.Count)
                throw new ArgumentException("Los cromosomas no tienen la misma longitud.");

            int cantidadCortes = Cromosoma.Count(gen => gen == 1);
            int cantidadCortesOtro = otro.Cromosoma.Count(gen => gen == 1);
            if (cantidadCortes != cantidadCortesOtro)
                throw new ArgumentException("Los cromosomas no tienen la misma cantidad de cortes.");

            int cantidadCortesSeleccionados = 0;
            var cortesDisponibles = new List<int>();
            var cromosomaHijo = Enumerable.Repeat(0, Cromosoma.Count).ToList<int>();

            for (int indice = 0; indice < Cromosoma.Count; indice++)
            {
                bool coinciden = Cromosoma[indice] == otro.Cromosoma[indice];
                if (coinciden)
                {
                    cromosomaHijo[indice] = Cromosoma[indice];
                    if (Cromosoma[indice] == 1)
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

            bool hijoIgualPadre = cromosomaHijo.SequenceEqual(Cromosoma);
            bool hijoIgualOtro = cromosomaHijo.SequenceEqual(otro.Cromosoma);
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

            var hijo = new IndividuoCortesBinarios(cromosomaHijo, _problema, _generadorRandom);
            return hijo;
        }

        internal override void Mutar()
        {
            List<int> porcionesOrdenadas = ObtenerPorcionesOrdenadasPorPreferencia();
            foreach (int indicePorcion in porcionesOrdenadas)
            {
                bool seAchico = AchicarPorcion(indicePorcion);
                if (seAchico)
                {
                    CalcularEstado();
                    return;
                }
            }
        }

        internal override decimal Fitness()
        {
            return _fitness;
        }

        private void ValidarCromosoma(List<int> cromosoma, InstanciaProblema problema)
        {
            int cantidadGenesEsperada = problema.CantidadAtomos - 1;
            if (cromosoma.Count != cantidadGenesEsperada)
            {
                string mensaje = $"Cantidad de genes inválida. Esperada: {cantidadGenesEsperada}, recibida: {cromosoma.Count}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }

            bool hayGenesInvalidos = cromosoma.Any(gen => gen != 0 && gen != 1);
            if (hayGenesInvalidos)
                throw new ArgumentException("El cromosoma solo puede contener genes 0 o 1.", nameof(cromosoma));

            int cantidadCortesEsperada = problema.Agentes.Count - 1;
            int cantidadCortes = cromosoma.Count(gen => gen == 1);
            if (cantidadCortes != cantidadCortesEsperada)
            {
                string mensaje = $"Cantidad de cortes inválida. Esperada: {cantidadCortesEsperada}, recibida: {cantidadCortes}";
                throw new ArgumentException(mensaje, nameof(cromosoma));
            }
        }

        private void CalcularEstado()
        {
            _posicionesCortes = [];
            for (int indice = 0; indice < Cromosoma.Count; indice++)
            {
                bool esCorte = Cromosoma[indice] == 1;
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
                bool pudoMover = AgrandarPorcionDerecha(indicePorcion);
                return pudoMover;
            }

            bool esUltimaPorcion = indicePorcion == _posicionesCortes.Count;
            if (esUltimaPorcion)
            {
                bool pudoMover = AgrandarPorcionIzquierda(indicePorcion);
                return pudoMover;
            }

            int preferenciaIzquierda = _preferenciasPorcion[indicePorcion - 1];
            int preferenciaDerecha = _preferenciasPorcion[indicePorcion + 1];

            bool estanEmpatadas = preferenciaIzquierda == preferenciaDerecha;
            if (estanEmpatadas)
            {
                int indiceGanador = _generadorRandom.Siguiente(2);
                bool agrandaIzquierda = indiceGanador == 0;
                bool pudoMover = agrandaIzquierda
                    ? AgrandarPorcionIzquierda(indicePorcion)
                    : AgrandarPorcionDerecha(indicePorcion);
                return pudoMover;
            }

            bool izquierdaEsMenosDeseada = preferenciaIzquierda < preferenciaDerecha;
            if (izquierdaEsMenosDeseada)
            {
                bool pudoMover = AgrandarPorcionIzquierda(indicePorcion);
                return pudoMover;
            }

            bool pudoMoverDerecha = AgrandarPorcionDerecha(indicePorcion);
            return pudoMoverDerecha;
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

        private bool AgrandarPorcionIzquierda(int indicePorcion)
        {
            int posicionActualDelCorte;
            int nuevaPosicionDelCorte;

            bool esUltimaPorcion = indicePorcion == _posicionesCortes.Count;
            if (esUltimaPorcion)
            {
                posicionActualDelCorte = _posicionesCortes[^1];
                nuevaPosicionDelCorte = posicionActualDelCorte + 1;
            }
            else
            {
                posicionActualDelCorte = _posicionesCortes[indicePorcion - 1];
                nuevaPosicionDelCorte = posicionActualDelCorte + 1;
            }

            bool pudoMover = MoverCorte(posicionActualDelCorte, nuevaPosicionDelCorte);
            return pudoMover;
        }

        private bool AgrandarPorcionDerecha(int indicePorcion)
        {
            int posicionActualDelCorte;
            int nuevaPosicionDelCorte;

            bool esPrimeraPorcion = indicePorcion == 0;
            if (esPrimeraPorcion)
            {
                posicionActualDelCorte = _posicionesCortes[0];
                nuevaPosicionDelCorte = posicionActualDelCorte - 1;
            }
            else
            {
                posicionActualDelCorte = _posicionesCortes[indicePorcion];
                nuevaPosicionDelCorte = posicionActualDelCorte - 1;
            }

            bool pudoMover = MoverCorte(posicionActualDelCorte, nuevaPosicionDelCorte);
            return pudoMover;
        }

        private bool MoverCorte(int posicionActual, int nuevaPosicion)
        {
            bool nuevaPosicionInvalida = nuevaPosicion < 1 || nuevaPosicion > Cromosoma.Count;
            if (nuevaPosicionInvalida)
                return false;

            int indiceActual = posicionActual - 1;
            bool posicionActualNoEsCorte = Cromosoma[indiceActual] == 0;
            if (posicionActualNoEsCorte)
                return false;

            int indiceNuevo = nuevaPosicion - 1;
            bool posicionNuevaYaEsCorte = Cromosoma[indiceNuevo] == 1;
            if (posicionNuevaYaEsCorte)
                return false;

            Cromosoma[indiceActual] = 0;
            Cromosoma[indiceNuevo] = 1;
            _posicionesCortes.Remove(posicionActual);
            _posicionesCortes.Add(nuevaPosicion);
            _posicionesCortes.Sort();
            return true;
        }
    }
}
