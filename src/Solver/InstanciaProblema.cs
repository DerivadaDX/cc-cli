namespace Solver
{
    internal class InstanciaProblema
    {
        private InstanciaProblema()
        {
        }

        internal List<Agente> Agentes { get; } = [];
        internal int CantidadAtomos => AtomosValorados.Count;
        private HashSet<int> AtomosValorados { get; } = [];

        internal static InstanciaProblema CrearDesdeMatrizDeValoraciones(decimal[,] matrizValoraciones)
        {
            ValidarMatriz(matrizValoraciones);

            var instanciaProblema = new InstanciaProblema();
            var agentesPorId = new Dictionary<int, Agente>();

            int cantidadFilas = matrizValoraciones.GetLength(0);
            int cantidadColumnas = matrizValoraciones.GetLength(1);

            for (int indiceAtomo = 0; indiceAtomo < cantidadFilas; indiceAtomo++)
            {
                bool atomoFueValorado = false;

                for (int indiceAgente = 0; indiceAgente < cantidadColumnas; indiceAgente++)
                {
                    int idAgente = indiceAgente + 1;
                    bool noExisteAgente = !agentesPorId.TryGetValue(idAgente, out Agente agente);
                    if (noExisteAgente)
                    {
                        agente = new Agente(idAgente);
                        agentesPorId[idAgente] = agente;
                        instanciaProblema.Agentes.Add(agente);
                    }

                    decimal valoracion = matrizValoraciones[indiceAtomo, indiceAgente];
                    if (valoracion > 0)
                    {
                        var atomo = new Atomo(indiceAtomo + 1, valoracion);
                        agente.AgregarValoracion(atomo);
                        atomoFueValorado = true;
                    }
                }

                if (atomoFueValorado)
                    instanciaProblema.AtomosValorados.Add(indiceAtomo + 1);
            }

            return instanciaProblema;
        }


        private static void ValidarMatriz(decimal[,] matrizValoraciones)
        {
            if (matrizValoraciones == null)
                throw new ArgumentException("La matriz de valoraciones no puede ser null", nameof(matrizValoraciones));

            int atomos = matrizValoraciones.GetLength(0);
            int agentes = matrizValoraciones.GetLength(1);

            if (atomos == 0 || agentes == 0)
                throw new ArgumentException("La matriz de valoraciones no puede estar vacía", nameof(matrizValoraciones));

            for (int indiceAgente = 0; indiceAgente < agentes; indiceAgente++)
            {
                bool agenteNoValoraNingunAtomo = true;
                for (int indiceAtomo = 0; indiceAtomo < atomos; indiceAtomo++)
                {
                    if (matrizValoraciones[indiceAtomo, indiceAgente] > 0)
                    {
                        agenteNoValoraNingunAtomo = false;
                        break;
                    }
                }

                if (agenteNoValoraNingunAtomo)
                {
                    throw new ArgumentException(
                        $"El agente {indiceAgente + 1} no tiene valoraciones positivas sobre ningún átomo.",
                        nameof(matrizValoraciones));
                }
            }
        }

    }
}
