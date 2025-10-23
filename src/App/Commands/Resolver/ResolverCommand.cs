using System.CommandLine;
using System.Diagnostics;
using Common;
using Solver;
using Solver.Individuos;

namespace App.Commands.Resolver
{
    internal class ResolverCommand
    {
        internal static Command Crear()
        {
            var command = new Command("resolver", "Resuelve una instancia");

            var rutaInstanciaArgument = new Argument<string>("ruta-instancia")
            {
                Description = "Ruta de la instancia a resolver",
            };
            var limiteGeneracionesOption = new Option<int>("--limite-generaciones", () => 0)
            {
                Description = "Límite de generaciones a computar (0 = infinito)",
            };
            var cantidadIndividuosOption = new Option<int>("--cantidad-individuos", () => 100)
            {
                Description = "Cantidad de individuos por generación",
            };
            var limiteEstancamientoOption = new Option<int>("--limite-estancamiento", () => 1000)
            {
                Description = "Límite de generaciones sin mejora (0 = infinito)",
            };
            var tipoIndividuoOption = new Option<string>("--tipo-individuo", () => "intercambio")
            {
                Description = "Tipo de individuo a utilizar (intercambio|optimizacion)",
            };
            var seedOption = new Option<int>("--seed", () => 0)
            {
                Description = "Semilla para la generación de números aleatorios",
            };

            command.AddArgument(rutaInstanciaArgument);
            command.AddOption(limiteGeneracionesOption);
            command.AddOption(cantidadIndividuosOption);
            command.AddOption(limiteEstancamientoOption);
            command.AddOption(tipoIndividuoOption);
            command.AddOption(seedOption);

            command.SetHandler(
                (rutaInstancia, limiteGeneraciones, cantidadIndividuos, limiteEstancamiento, tipoIndividuo, seed) =>
                {
                    var tipoIndividuos = TipoIndividuoHelper.Parse(tipoIndividuo);
                    var parametros = new ParametrosSolucion
                    {
                        RutaInstancia = rutaInstancia,
                        LimiteGeneraciones = limiteGeneraciones,
                        CantidadIndividuos = cantidadIndividuos,
                        LimiteEstancamiento = limiteEstancamiento,
                        TipoIndividuos = tipoIndividuos,
                    };

                    var fileSystemHelper = FileSystemHelperFactory.Crear();
                    var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                    var consola = ConsoleProxyFactory.Crear();
                    var presentador = new Presentador(consola);

                    var generadorRandom = GeneradorNumerosRandomFactory.Crear(seed);
                    EjecutarResolucion(parametros, lector, presentador, generadorRandom);

#if DEBUG
                    Console.WriteLine("Presioná una tecla para salir...");
                    Console.ReadKey();
#endif
                },
                rutaInstanciaArgument,
                limiteGeneracionesOption,
                cantidadIndividuosOption,
                limiteEstancamientoOption,
                tipoIndividuoOption,
                seedOption
            );

            return command;
        }

        internal static void EjecutarResolucion(
            ParametrosSolucion parametros,
            LectorArchivoMatrizValoraciones lector,
            Presentador presentador,
            GeneradorNumerosRandom generadorRandom
        )
        {
            using var cts = new CancellationTokenSource();
            ConfigurarCancelacion(cts, presentador);

            try
            {
                decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var poblacion = PoblacionFactory.Crear(
                    parametros.CantidadIndividuos,
                    instanciaProblema,
                    parametros.TipoIndividuos,
                    generadorRandom
                );

                var algoritmoGenetico = new AlgoritmoGenetico(
                    poblacion,
                    parametros.LimiteGeneraciones,
                    parametros.LimiteEstancamiento
                );
                ConfigurarProgreso(parametros, algoritmoGenetico, presentador);
                ConfigurarEstancamiento(algoritmoGenetico, presentador);

                var stopwatch = Stopwatch.StartNew();
                var (mejorIndividuo, generaciones) = algoritmoGenetico.Ejecutar(cts.Token);
                stopwatch.Stop();

                MostrarResultado(mejorIndividuo, generaciones, stopwatch.ElapsedMilliseconds, presentador);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message, presentador);
            }
        }

        private static void ConfigurarCancelacion(CancellationTokenSource cts, Presentador presentador)
        {
            Console.CancelKeyPress += (_, e) =>
            {
                presentador.MostrarAdvertencia("Cancelación solicitada por el usuario.");
                e.Cancel = true;
                cts.Cancel();
            };
        }

        private static void ConfigurarProgreso(
            ParametrosSolucion parametros,
            AlgoritmoGenetico algoritmoGenetico,
            Presentador presentador
        )
        {
            if (parametros.LimiteGeneraciones > 0)
            {
                const int tamañoBarraProgreso = 50;
                algoritmoGenetico.GeneracionProcesada += (generacion, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    int progreso = generacion * tamañoBarraProgreso / parametros.LimiteGeneraciones;
                    string barraProgreso = new string('#', progreso).PadRight(tamañoBarraProgreso, '-');
                    string mensaje = $"[{barraProgreso}] {generacion}/{parametros.LimiteGeneraciones}";
                    presentador.MostrarProgreso(mensaje);
                };
            }
            else
            {
                algoritmoGenetico.GeneracionProcesada += (generacion, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    string mensaje = $"Procesando generación #{generacion}.";
                    presentador.MostrarProgreso(mensaje);
                };
            }
        }

        private static void ConfigurarEstancamiento(AlgoritmoGenetico algoritmoGenetico, Presentador presentador)
        {
            algoritmoGenetico.EstancamientoDetectado += () =>
            {
                presentador.MostrarAdvertencia("Procesamiento detenido por estancamiento.");
            };
        }

        private static void MostrarResultado(Individuo mejorIndividuo, int generaciones, long tiempoMs, Presentador presentador)
        {
            presentador.MostrarExito($"Resultado encontrado después de {generaciones} generaciones.");
            presentador.MostrarExito($"Resultado obtenido: {mejorIndividuo}.");
            presentador.MostrarExito($"Tiempo de ejecución: {tiempoMs} ms.");
        }

        private static void MostrarError(string mensaje, Presentador presentador)
        {
            presentador.MostrarError($"Se produjo un error: {mensaje}");
        }
    }
}
