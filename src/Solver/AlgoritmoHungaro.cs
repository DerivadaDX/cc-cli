using HungarianAlgorithmSolver = HungarianAlgorithm.HungarianAlgorithm;

internal class AlgoritmoHungaro
{
    public int[] CalcularAsignacionOptimaDePorciones(decimal[,] valoracionesDePorciones)
    {
        int[,] matrizCostosEnteros = ConvertirValoracionesACostos(valoracionesDePorciones);
        int[] asignaciones = HungarianAlgorithmSolver.FindAssignments(matrizCostosEnteros);
        return asignaciones;
    }

    private int[,] ConvertirValoracionesACostos(decimal[,] valoracionesDePorciones)
    {
        decimal valoracionMaxima = ObtenerValoracionMaxima(valoracionesDePorciones);
        int cantidadPorciones = valoracionesDePorciones.GetLength(0);
        int cantidadAgentes = valoracionesDePorciones.GetLength(1);

        int[,] costos = new int[cantidadPorciones, cantidadAgentes];
        for (int indicePorcion = 0; indicePorcion < cantidadPorciones; indicePorcion++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal costo = valoracionMaxima - valoracionesDePorciones[indicePorcion, indiceAgente];

                // TODO: Poder trabajar con decimal. Se pasa a int porque la biblioteca que usamos no soporta decimal.
                int costoRedondeado = (int)Math.Round(costo * 1000m);
                costos[indicePorcion, indiceAgente] = costoRedondeado;
            }
        }

        return costos;
    }

    private decimal ObtenerValoracionMaxima(decimal[,] valoraciones)
    {
        int cantidadPorciones = valoraciones.GetLength(0);
        int cantidadAgentes = valoraciones.GetLength(1);

        var valoracionMaxima = decimal.MinValue;
        for (int indicePorcion = 0; indicePorcion < cantidadPorciones; indicePorcion++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal valorActual = valoraciones[indicePorcion, indiceAgente];
                if (valorActual > valoracionMaxima)
                    valoracionMaxima = valorActual;
            }
        }

        return valoracionMaxima;
    }
}
