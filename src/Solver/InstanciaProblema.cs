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

        internal static InstanciaProblema CrearDesdeMatrizDeValoraciones(decimal[][] matrizValoraciones)
        {
            ValidarMatriz(matrizValoraciones);

            var instanciaProblema = new InstanciaProblema();

            var agentesPorId = new Dictionary<int, Agente>();
            for (int indiceAtomo = 0; indiceAtomo < matrizValoraciones.Length; indiceAtomo++)
            {
                bool atomoFueValorado = false;

                for (int indiceAgente = 0; indiceAgente < matrizValoraciones[indiceAtomo].Length; indiceAgente++)
                {
                    int idAgente = indiceAgente + 1;
                    bool noExisteAgente = !agentesPorId.TryGetValue(idAgente, out Agente agente);
                    if (noExisteAgente)
                    {
                        agente = new Agente(idAgente);
                        agentesPorId[idAgente] = agente;
                        instanciaProblema.Agentes.Add(agente);
                    }

                    decimal valoracion = matrizValoraciones[indiceAtomo][indiceAgente];
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

        private static void ValidarMatriz(decimal[][] matrizValoraciones)
        {
            if (matrizValoraciones == null)
                throw new ArgumentException("La matriz de valoraciones no puede ser null", nameof(matrizValoraciones));

            if (matrizValoraciones.Length == 0)
                throw new ArgumentException("La matriz de valoraciones no puede estar vacía", nameof(matrizValoraciones));

            int longitudPrimeraFila = matrizValoraciones[0].Length;
            for (int fila = 1; fila < matrizValoraciones.Length; fila++)
            {
                if (matrizValoraciones[fila].Length != longitudPrimeraFila)
                    throw new ArgumentException("Todas las filas de la matriz deben tener la misma longitud", nameof(matrizValoraciones));
            }

            for (int indiceAgente = 0; indiceAgente < matrizValoraciones[0].Length; indiceAgente++)
            {
                bool agenteNoValoraNingunAtomo = matrizValoraciones.All(fila => fila[indiceAgente] <= 0);
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
