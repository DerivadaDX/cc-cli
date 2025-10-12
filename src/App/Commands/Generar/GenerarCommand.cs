using System.CommandLine;
using Common;
using Generator;

namespace App.Commands.Generar
{
    internal static class GenerarCommand
    {
        internal const int ValorMaximoPorDefecto = 1000;
        internal const string RutaSalidaPorDefecto = "instancia.dat";

        internal static Command Crear()
        {
            var command = new Command("generar", "Genera una nueva instancia");

            var rutaSalidaArgument = new Argument<string>("ruta-salida", () => RutaSalidaPorDefecto)
            {
                Description = "Ruta donde guardar la instancia generada",
            };
            var atomosOption = new Option<int>("--atomos") { Description = "Cantidad de átomos", IsRequired = true };
            var agentesOption = new Option<int>("--agentes") { Description = "Cantidad de agentes", IsRequired = true };
            var valorMaximoOption = new Option<int>("--valor-maximo", () => ValorMaximoPorDefecto)
            {
                Description = "Valor máximo para cada valoración",
            };
            var disjuntasOption = new Option<bool>("--disjuntas") { Description = "Indica si las valoraciones son disjuntas" };
            var seedOption = new Option<int?>("--seed") { Description = "Semilla para la generación de números aleatorios" };

            command.AddArgument(rutaSalidaArgument);
            command.AddOption(atomosOption);
            command.AddOption(agentesOption);
            command.AddOption(valorMaximoOption);
            command.AddOption(disjuntasOption);
            command.AddOption(seedOption);

            command.SetHandler(
                (rutaSalida, atomos, agentes, valorMaximo, disjuntas, seed) =>
                {
                    var parametros = new ParametrosGeneracion
                    {
                        RutaSalida = rutaSalida,
                        Atomos = atomos,
                        Agentes = agentes,
                        ValorMaximo = valorMaximo,
                        ValoracionesDisjuntas = disjuntas,
                    };

                    if (!seed.HasValue)
                        seed = GeneradorNumerosRandom.GenerarSeed();

                    var generadorNumerosRandom = GeneradorNumerosRandomFactory.Crear(seed.Value);
                    var builder = new InstanciaBuilder(generadorNumerosRandom);

                    var fileSystemHelper = FileSystemHelperFactory.Crear();
                    var escritor = new EscritorInstancia(fileSystemHelper);

                    var consola = ConsoleProxyFactory.Crear();
                    var presentador = new Presentador(consola);

                    EjecutarGeneracion(parametros, builder, escritor, presentador);
                },
                rutaSalidaArgument,
                atomosOption,
                agentesOption,
                valorMaximoOption,
                disjuntasOption,
                seedOption
            );

            return command;
        }

        internal static void EjecutarGeneracion(
            ParametrosGeneracion parametros,
            InstanciaBuilder builder,
            EscritorInstancia escritor,
            Presentador presentador
        )
        {
            try
            {
                presentador.MostrarInfo($"Usando seed '{parametros.Seed}'");

                decimal[,] instancia = builder
                    .ConCantidadDeAtomos(parametros.Atomos)
                    .ConCantidadDeAgentes(parametros.Agentes)
                    .ConValorMaximo(parametros.ValorMaximo)
                    .ConValoracionesDisjuntas(parametros.ValoracionesDisjuntas)
                    .Build();

                escritor.EscribirInstancia(instancia, parametros.RutaSalida);
                presentador.MostrarExito($"Instancia generada y guardada en '{parametros.RutaSalida}'.");
            }
            catch (Exception ex)
            {
                presentador.MostrarError($"Error al generar la instancia: {ex.Message}");
            }
        }
    }
}
