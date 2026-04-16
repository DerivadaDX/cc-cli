using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver;

internal class AlgoritmoHungaro
{
    public virtual List<int> CalcularAsignacionOptimaDePorciones(decimal[,] valoracionesDePorciones)
    {
        ArgumentNullException.ThrowIfNull(valoracionesDePorciones, nameof(valoracionesDePorciones));

        int cantidadPorciones = valoracionesDePorciones.GetLength(0);
        int cantidadAgentes = valoracionesDePorciones.GetLength(1);
        if (cantidadPorciones != cantidadAgentes)
            throw new ArgumentException("La matriz de valoraciones debe ser cuadrada.", nameof(valoracionesDePorciones));

        decimal[,] costos = ConvertirValoracionesACostos(valoracionesDePorciones);
        int[] asignaciones = ResolverAsignacionesMinimizandoCostos(costos);

        List<int> resultado = [.. asignaciones];
        return resultado;
    }

    private static decimal[,] ConvertirValoracionesACostos(decimal[,] valoracionesDePorciones)
    {
        decimal valoracionMaxima = ObtenerValoracionMaxima(valoracionesDePorciones);
        int cantidadPorciones = valoracionesDePorciones.GetLength(0);
        int cantidadAgentes = valoracionesDePorciones.GetLength(1);

        var costos = new decimal[cantidadPorciones, cantidadAgentes];
        for (int indicePorcion = 0; indicePorcion < cantidadPorciones; indicePorcion++)
        {
            for (int indiceAgente = 0; indiceAgente < cantidadAgentes; indiceAgente++)
            {
                decimal valoracion = valoracionesDePorciones[indicePorcion, indiceAgente];
                decimal costo = valoracionMaxima - valoracion;
                costos[indicePorcion, indiceAgente] = costo;
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

    private static int[] ResolverAsignacionesMinimizandoCostos(decimal[,] costos)
    {
        int cantidadFilas = costos.GetLength(0);
        int cantidadColumnas = costos.GetLength(1);

        int dimension = cantidadFilas;
        if (dimension == 0)
            return [];

        decimal[] potencialFilas = new decimal[dimension + 1];
        decimal[] potencialColumnas = new decimal[dimension + 1];
        int[] columnaEmparejadaPorFila = new int[dimension + 1];
        int[] camino = new int[dimension + 1];

        for (int fila = 1; fila <= dimension; fila++)
        {
            columnaEmparejadaPorFila[0] = fila;

            int columnaActual = 0;
            decimal[] costosMinimos = Enumerable.Repeat(decimal.MaxValue, dimension + 1).ToArray();
            bool[] columnaUsada = new bool[dimension + 1];

            while (true)
            {
                columnaUsada[columnaActual] = true;
                int filaActual = columnaEmparejadaPorFila[columnaActual];
                decimal delta = decimal.MaxValue;
                int siguienteColumna = 0;
                decimal potencialFila = potencialFilas[filaActual];

                for (int columna = 1; columna <= dimension; columna++)
                {
                    if (columnaUsada[columna])
                        continue;

                    decimal costoOriginal = costos[filaActual - 1, columna - 1];
                    decimal potencialColumna = potencialColumnas[columna];
                    decimal costoReducido = costoOriginal - potencialFila - potencialColumna;
                    decimal costoMinimoActual = costosMinimos[columna];
                    if (costoReducido < costoMinimoActual)
                    {
                        costoMinimoActual = costoReducido;
                        costosMinimos[columna] = costoMinimoActual;
                        camino[columna] = columnaActual;
                    }

                    if (costoMinimoActual < delta)
                    {
                        delta = costoMinimoActual;
                        siguienteColumna = columna;
                    }
                }

                for (int columna = 0; columna <= dimension; columna++)
                {
                    bool estaUsada = columnaUsada[columna];
                    if (estaUsada)
                    {
                        int filaEmparejada = columnaEmparejadaPorFila[columna];
                        potencialFilas[filaEmparejada] += delta;
                        potencialColumnas[columna] -= delta;
                    }
                    else
                    {
                        decimal costoMinimoActual = costosMinimos[columna];
                        costosMinimos[columna] = costoMinimoActual - delta;
                    }
                }

                columnaActual = siguienteColumna;

                if (columnaEmparejadaPorFila[columnaActual] == 0)
                    break;
            }

            while (columnaActual != 0)
            {
                int columnaAnterior = camino[columnaActual];
                columnaEmparejadaPorFila[columnaActual] = columnaEmparejadaPorFila[columnaAnterior];
                columnaActual = columnaAnterior;
            }
        }

        int[] asignaciones = new int[dimension];
        for (int columna = 1; columna <= dimension; columna++)
        {
            int fila = columnaEmparejadaPorFila[columna];
            asignaciones[fila - 1] = columna - 1;
        }

        return asignaciones;
    }
}
