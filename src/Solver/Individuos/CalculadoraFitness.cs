namespace Solver.Individuos
{
    internal class CalculadoraFitness
    {
        public decimal CalcularFitness(Individuo individuo, InstanciaProblema problema)
        {
            decimal violacionTotal = 0;
            List<Agente> agentes = individuo.ExtraerAsignacion();

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
