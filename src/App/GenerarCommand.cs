using System.CommandLine;
using Common;
using GeneradorInstancia;

namespace App
{
    internal static class GenerarCommand
    {
        internal static Command Create()
        {
            var command = new Command("generar", "Genera una nueva instancia");

            var atomosOption = new Option<int>("--atomos") { IsRequired = true };
            var agentesOption = new Option<int>("--agentes") { IsRequired = true };
            var valorMaximoOption = new Option<int>("--valor-maximo", () => 1000);
            var outputOption = new Option<string>("--output", () => "instancia.dat");
            var disjuntasOption = new Option<bool>("--disjuntas");

            command.AddOption(atomosOption);
            command.AddOption(agentesOption);
            command.AddOption(valorMaximoOption);
            command.AddOption(outputOption);
            command.AddOption(disjuntasOption);

            command.SetHandler((atomos, agentes, max, output, disjuntas) =>
            {
                var parametros = new ParametrosGeneracion(atomos, agentes, max, output, disjuntas);
                var builder = new InstanciaBuilder();
                var escritor = new EscritorInstancia(new FileSystemHelper());
                Handler(parametros, builder, escritor);
            }, atomosOption, agentesOption, valorMaximoOption, outputOption, disjuntasOption);

            return command;
        }

        internal static void Handler(ParametrosGeneracion parametros, InstanciaBuilder builder, EscritorInstancia escritor)
        {
            builder
                .ConCantidadDeAtomos(parametros.Atomos)
                .ConCantidadDeJugadores(parametros.Agentes)
                .ConValorMaximo(parametros.ValorMaximo)
                .ConValoracionesDisjuntas(parametros.ValoracionesDisjuntas);

            escritor.EscribirInstancia(builder.Build(), parametros.RutaSalida);
        }
    }
}
