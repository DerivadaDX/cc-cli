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
            var instanciaOption = new Option<string>("--instancia")
            {
                Description = "Ruta de la instancia a resolver",
                IsRequired = true,
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

            command.AddOption(instanciaOption);
            command.AddOption(limiteGeneracionesOption);
            command.AddOption(cantidadIndividuosOption);
            command.AddOption(limiteEstancamientoOption);
            command.AddOption(tipoIndividuoOption);

            command.SetHandler((rutaInstancia, limiteGeneraciones, cantidadIndividuos, limiteEstancamiento, tipoIndividuoStr) =>
            {
                var tipoIndividuo = TipoIndividuoHelper.Parse(tipoIndividuoStr);
                var parametros = new ParametrosSolucion(rutaInstancia, limiteGeneraciones, cantidadIndividuos, limiteEstancamiento, tipoIndividuo);

                var fileSystemHelper = FileSystemHelperFactory.Crear();
                var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                var consola = ConsoleProxyFactory.Crear();
                var presentador = new Presentador(consola);

                EjecutarResolucion(parametros, lector, presentador);

#if DEBUG
                Console.WriteLine("Presioná una tecla para salir...");
                Console.ReadKey();
#endif
            }, instanciaOption, limiteGeneracionesOption, cantidadIndividuosOption, limiteEstancamientoOption, tipoIndividuoOption);

            return command;
        }

        internal static void EjecutarResolucion(
            ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector, Presentador presentador)
        {
            using var cts = new CancellationTokenSource();
            ConfigurarCancelacion(cts, presentador);

            try
            {
                decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                IIndividuoFactory individuoFactory = ObtenerIndividuoFactory(parametros, presentador, instanciaProblema);

                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones, parametros.LimiteEstancamiento);
                ConfigurarProgreso(parametros, algoritmoGenetico, presentador);

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

        private static IIndividuoFactory ObtenerIndividuoFactory(
            ParametrosSolucion parametros, Presentador presentador, InstanciaProblema instanciaProblema)
        {
            IIndividuoFactory individuoFactory = parametros.TipoIndividuo switch
            {
                TipoIndividuo.Intercambio => new IndividuoIntercambioAsignacionesFactory(instanciaProblema),
                TipoIndividuo.Optimizacion => new IndividuoOptimizacionAsignacionesFactory(instanciaProblema),
                _ => throw new ArgumentException($"Tipo de individuo no soportado: {parametros.TipoIndividuo}")
            };

            presentador.MostrarInfo($"Utilizando individuos de tipo: {parametros.TipoIndividuo}");
            return individuoFactory;
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
            ParametrosSolucion parametros, AlgoritmoGenetico algoritmoGenetico, Presentador presentador)
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

        private static void MostrarResultado(
            Individuo mejorIndividuo, int generaciones, long tiempoMs, Presentador presentador)
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
