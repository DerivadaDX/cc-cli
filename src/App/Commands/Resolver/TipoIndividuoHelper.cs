using System;
using Solver.Individuos;

namespace App.Commands.Resolver
{
    internal static class TipoIndividuoHelper
    {
        internal static TipoIndividuo Parse(string valor)
        {
            ArgumentNullException.ThrowIfNull(valor, nameof(valor));

            TipoIndividuo tipo = valor.ToLowerInvariant() switch
            {
                "intercambio" => TipoIndividuo.IntercambioAsignaciones,
                "optimizacion" => TipoIndividuo.OptimizacionAsignaciones,
                "cortes-binarios" => TipoIndividuo.CortesBinarios,
                _ => throw new ArgumentException($"Tipo de individuo '{valor}' no reconocido.", nameof(valor)),
            };

            return tipo;
        }
    }
}
