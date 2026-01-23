namespace Solver
{
    internal class CalculadoraValoracionesPorciones
    {
        public virtual decimal[,] CalcularMatrizValoracionesPorcionAgente(
            InstanciaProblema problema, IReadOnlyList<int> posicionesCortes)
        {
            ArgumentNullException.ThrowIfNull(problema, nameof(problema));
            ArgumentNullException.ThrowIfNull(posicionesCortes, nameof(posicionesCortes));

            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortesEsperada = cantidadAgentes - 1;
            if (posicionesCortes.Count != cantidadCortesEsperada)
            {
                string mensajeTemplate = "La cantidad de cortes indicada no coincide con la esperada ({0}). Recibida: {1}";
                string mensaje = string.Format(mensajeTemplate, cantidadCortesEsperada, posicionesCortes.Count);
                throw new ArgumentException(mensaje, nameof(posicionesCortes));
            }

            var cortesOrdenados = posicionesCortes.OrderBy(x => x).ToList<int>();
            decimal[,] valoraciones = new decimal[cantidadAgentes, cantidadAgentes];

            int atomoInicio = 1;
            for (int indicePorcion = 0; indicePorcion < cantidadAgentes; indicePorcion++)
            {
                int atomoFin = indicePorcion < cortesOrdenados.Count
                    ? cortesOrdenados[indicePorcion]
                    : problema.CantidadAtomos;

                for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
                {
                    Agente agente = problema.Agentes[indiceAgente];
                    decimal valorPorcion = CalcularValorPorcion(agente, atomoInicio, atomoFin);
                    valoraciones[indicePorcion, indiceAgente] = valorPorcion;
                }

                atomoInicio = atomoFin + 1;
            }

            return valoraciones;
        }

        public virtual List<int> CalcularPreferenciasPorcion(decimal[,] valoracionesDePorciones)
        {
            ArgumentNullException.ThrowIfNull(valoracionesDePorciones, nameof(valoracionesDePorciones));

            int cantidadPorciones = valoracionesDePorciones.GetLength(0);
            int cantidadAgentes = valoracionesDePorciones.GetLength(1);

            var preferencias = Enumerable.Repeat(0, cantidadPorciones).ToList<int>();
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal valorMaximo = decimal.MinValue;
                List<int> porcionesFavoritas = [];

                for (int indicePorcion = 0; indicePorcion < cantidadPorciones; indicePorcion++)
                {
                    decimal valoracion = valoracionesDePorciones[indicePorcion, indiceAgente];
                    if (valoracion > valorMaximo)
                    {
                        valorMaximo = valoracion;
                        porcionesFavoritas.Clear();
                        porcionesFavoritas.Add(indicePorcion);
                    }
                    else if (valoracion == valorMaximo)
                    {
                        porcionesFavoritas.Add(indicePorcion);
                    }
                }

                foreach (int porcion in porcionesFavoritas)
                    preferencias[porcion]++;
            }

            return preferencias;
        }

        private decimal CalcularValorPorcion(Agente agente, int atomoInicio, int atomoFin)
        {
            decimal valor = 0;
            foreach (Atomo atomo in agente.Valoraciones)
            {
                if (atomo.Posicion < atomoInicio || atomo.Posicion > atomoFin)
                    continue;

                valor += atomo.Valoracion;
            }

            return valor;
        }
    }
}
