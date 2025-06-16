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
            try
            {
                using var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\r" + new string(' ', Console.BufferWidth - 1));
                    Console.WriteLine("\rCancelación solicitada por el usuario.");
                    Console.ResetColor();
                    e.Cancel = true;
                    cts.Cancel();
                };

                decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);
                if (parametros.LimiteGeneraciones > 0)
                {
                    const int tamañoBarraProgreso = 50;
                    algoritmoGenetico.GeneracionProcesada += generacion =>
                    {
                        if (cts.IsCancellationRequested) return;

                        int progreso = generacion * tamañoBarraProgreso / parametros.LimiteGeneraciones;
                        string barra = new string('#', progreso).PadRight(tamañoBarraProgreso, '-');
                        Console.Write($"\r[{barra}] {generacion}/{parametros.LimiteGeneraciones}");
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

                var stopwatch = Stopwatch.StartNew();
                (Individuo mejorIndividuo, int generaciones) = algoritmoGenetico.Ejecutar(cts.Token);
                stopwatch.Stop();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Resultado encontrado después de {generaciones} generaciones.");
                Console.WriteLine($"Resultado obtendio: {mejorIndividuo}.");
                Console.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds} ms.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nSe produjo un error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
