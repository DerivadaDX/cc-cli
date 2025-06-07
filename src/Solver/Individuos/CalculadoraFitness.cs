namespace Solver.Individuos
{
    internal class CalculadoraFitness
    {
        public decimal CalcularFitness(Individuo individuo, InstanciaProblema problema)
        {
            decimal violacionTotal = 0;

            List<Agente> agentes = ExtraerAsignacion(individuo, problema);
            foreach (Agente agenteAsignado in agentes)
            {
                decimal maxViolacion = 0;

                decimal valoracionPropia = agenteAsignado.Valoraciones.Sum(v => v.Valoracion);
                foreach (Agente otroAgente in agentes)
                {
                    if (otroAgente.Id == agenteAsignado.Id)
                        continue;

                    decimal valoracionAjena = CalcularValoracionAjena(agenteAsignado, otroAgente, problema);
                    if (valoracionAjena <= valoracionPropia)
                        continue;

                    decimal violacion = valoracionAjena - valoracionPropia;
                    if (violacion > maxViolacion)
                        maxViolacion = violacion;
                }

                violacionTotal += maxViolacion;
            }

            return violacionTotal;
        }

        private List<Agente> ExtraerAsignacion(Individuo individuo, InstanciaProblema problema)
        {
            int cantidadAgentes = problema.Agentes.Count;
            int cantidadCortes = cantidadAgentes - 1;

            var cortes = individuo.Cromosoma.Take(cantidadCortes).ToList<int>();
            var asignaciones = individuo.Cromosoma.Skip(cantidadCortes).ToList<int>();
            cortes.Sort();

            Dictionary<int, Agente> agentesClonados = problema.Agentes
                .Select(a => new Agente(a.Id))
                .ToDictionary(a => a.Id);

            int atomoInicio = 1;
            for (int i = 0; i < cantidadAgentes; i++)
            {
                int atomoFin = (i < cortes.Count) ? cortes[i] : problema.CantidadAtomos;
                int idAgente = asignaciones[i];

                Agente agente = agentesClonados[idAgente];
                for (int pos = atomoInicio; pos <= atomoFin; pos++)
                {
                    Atomo atomoValorado = problema.Agentes
                        .First(a => a.Id == idAgente).Valoraciones
                        .FirstOrDefault(v => v.Posicion == pos);

                    decimal valoracion = atomoValorado?.Valoracion ?? 0;
                    agente.AgregarValoracion(new Atomo(pos, valoracion));
                }
                atomoInicio = atomoFin + 1;
            }

            var resultado = agentesClonados.Values.ToList<Agente>();
            return resultado;
        }

        private decimal CalcularValoracionAjena(Agente agenteValuador, Agente otroAgente, InstanciaProblema problema)
        {
            decimal resultado = 0;

            Agente agenteOriginal = problema.Agentes.First(a => a.Id == agenteValuador.Id);
            foreach (Atomo atomoAjeno in otroAgente.Valoraciones)
            {
                bool agenteOriginalValuaEsteAtomo = agenteOriginal.Valoraciones.Any(a => a.Posicion == atomoAjeno.Posicion);
                if (!agenteOriginalValuaEsteAtomo)
                    continue;

                Atomo atomoValuado = agenteOriginal.Valoraciones.First(a => a.Posicion == atomoAjeno.Posicion);
                resultado += atomoValuado.Valoracion;
            }

            return resultado;
        }
    }
}
