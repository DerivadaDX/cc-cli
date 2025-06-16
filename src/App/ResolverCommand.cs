using System.CommandLine;
using System.Diagnostics;
using Common;
using Solver;
using Solver.Individuos;

namespace App
{
    internal class ResolverCommand
    {
        internal static Command Create()
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

            command.AddOption(instanciaOption);
            command.AddOption(limiteGeneracionesOption);
            command.AddOption(cantidadIndividuosOption);

            command.SetHandler((rutaInstancia, limiteGeneraciones, cantidadIndividuos) =>
            {
                var parametros = new ParametrosSolucion(rutaInstancia, limiteGeneraciones, cantidadIndividuos);

                var fileSystemHelper = FileSystemHelperFactory.Crear();
                var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                Handler(parametros, lector);

#if DEBUG
                Console.WriteLine("Presioná una tecla para salir...");
                Console.ReadKey();
#endif
            }, instanciaOption, limiteGeneracionesOption, cantidadIndividuosOption);

            return command;
        }

        internal static void Handler(ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector)
        {
            using var cts = new CancellationTokenSource();
            ConfigurarCancelacion(cts);

            try
            {
                var matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);
                ConfigurarProgreso(algoritmoGenetico, parametros, cts);

                var stopwatch = Stopwatch.StartNew();
                var (mejorIndividuo, generaciones) = algoritmoGenetico.Ejecutar(cts.Token);
                stopwatch.Stop();

                MostrarResultado(mejorIndividuo, generaciones, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private static void ConfigurarCancelacion(CancellationTokenSource cts)
        {
            Console.CancelKeyPress += (_, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\r" + new string(' ', Console.BufferWidth - 1));
                Console.WriteLine("\rCancelación solicitada por el usuario.");
                Console.ResetColor();
                e.Cancel = true;
                cts.Cancel();
            };
        }

        private static void ConfigurarProgreso(
            AlgoritmoGenetico algoritmoGenetico, ParametrosSolucion parametros, CancellationTokenSource cts)
        {
            if (parametros.LimiteGeneraciones > 0)
            {
                const int tamañoBarraProgreso = 50;
                algoritmoGenetico.GeneracionProcesada += generacion =>
                {
                    if (cts.IsCancellationRequested) return;

                    int progreso = generacion * tamañoBarraProgreso / parametros.LimiteGeneraciones;
                    string barraProgreso = new string('#', progreso).PadRight(tamañoBarraProgreso, '-');
                    Console.Write($"\r[{barraProgreso}] {generacion}/{parametros.LimiteGeneraciones}");
                };
            }
            else
            {
                algoritmoGenetico.GeneracionProcesada += generacion =>
                {
                    if (cts.IsCancellationRequested) return;
                    Console.Write($"\rProcesando generación #{generacion}.");
                };
            }
        }

        private static void MostrarResultado(Individuo mejorIndividuo, int generaciones, long tiempoMs)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Resultado encontrado después de {generaciones} generaciones.");
            Console.WriteLine($"Resultado obtendio: {mejorIndividuo}.");
            Console.WriteLine($"Tiempo de ejecución: {tiempoMs} ms.");
            Console.ResetColor();
        }

        private static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nSe produjo un error: {mensaje}");
            Console.ResetColor();
        }
    }
}
