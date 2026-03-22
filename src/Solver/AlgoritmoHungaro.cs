using System;
using System.Collections.Generic;
using HungarianAlgorithmSolver = HungarianAlgorithm.HungarianAlgorithm;

namespace Solver;

internal class AlgoritmoHungaro
{
    public virtual List<int> CalcularAsignacionOptimaDePorciones(decimal[,] valoracionesDePorciones)
    {
        int[,] matrizCostosEnteros = ConvertirValoracionesACostos(valoracionesDePorciones);
        int[] asignaciones = HungarianAlgorithmSolver.FindAssignments(matrizCostosEnteros);

        List<int> resultado = [.. asignaciones];
        return resultado;
    }

    private static int[,] ConvertirValoracionesACostos(decimal[,] valoracionesDePorciones)
    {
        decimal valoracionMaxima = ObtenerValoracionMaxima(valoracionesDePorciones);
        int cantidadPorciones = valoracionesDePorciones.GetLength(0);
        int cantidadAgentes = valoracionesDePorciones.GetLength(1);

        int[,] costos = new int[cantidadPorciones, cantidadAgentes];
        for (int indicePorcion = 0; indicePorcion < cantidadPorciones; indicePorcion++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal valoracion = valoracionesDePorciones[indicePorcion, indiceAgente];
                decimal costo = valoracionMaxima - valoracion;

                int costoRedondeado = (int)Math.Round(costo * 1000m);
                costos[indicePorcion, indiceAgente] = costoRedondeado;
            }
        }

        return costos;
    }

    private static decimal ObtenerValoracionMaxima(decimal[,] valoraciones)
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
