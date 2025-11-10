using HungarianAlgorithmSolver = HungarianAlgorithm.HungarianAlgorithm;

internal class AlgoritmoHungaro
{
    public int[] EncontrarAsignacionQueMinimizaEnvidia(decimal[,] valoraciones)
    {
        int[,] matrizCostosEnteros = ConvertirValoracionesACostos(valoraciones);
        int[] asignaciones = HungarianAlgorithmSolver.FindAssignments(matrizCostosEnteros);
        return asignaciones;
    }

    private int[,] ConvertirValoracionesACostos(decimal[,] valoraciones)
    {
        decimal valoracionMaxima = ObtenerValoracionMaxima(valoraciones);
        int cantidadAtomos = valoraciones.GetLength(0);
        int cantidadAgentes = valoraciones.GetLength(1);

        int[,] costos = new int[cantidadAtomos, cantidadAgentes];
        for (int indiceAtomo = 0; indiceAtomo < cantidadAtomos; indiceAtomo++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal costo = valoracionMaxima - valoraciones[indiceAtomo, indiceAgente];

                // TODO: Poder trabajar con decimal. Se pasa a int porque la biblioteca que usamos no soporta decimal.
                int costoRedondeado = (int)Math.Round(costo * 1000m);
                costos[indiceAtomo, indiceAgente] = costoRedondeado;
            }
        }

        return costos;
    }

    private decimal ObtenerValoracionMaxima(decimal[,] valoraciones)
    {
        int cantidadAtomos = valoraciones.GetLength(0);
        int cantidadAgentes = valoraciones.GetLength(1);

        var valoracionMaxima = decimal.MinValue;
        for (int indiceAtomo = 0; indiceAtomo < cantidadAtomos; indiceAtomo++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal valorActual = valoraciones[indiceAtomo, indiceAgente];
                if (valorActual > valoracionMaxima)
                    valoracionMaxima = valorActual;
            }
        }

        return valoracionMaxima;
    }
}
