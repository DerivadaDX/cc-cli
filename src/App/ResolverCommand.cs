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
            var instanciaOption = new Option<string>("--instancia") { IsRequired = true };
            var limiteGeneracionesOption = new Option<int>("--limite-generaciones", () => 0);
            var cantidadIndividuosOption = new Option<int>("--cantidad-individuos", () => 100);

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
                decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);
                if (parametros.LimiteGeneraciones > 0)
                {
                    const int tamañoBarra = 50;
                    algoritmoGenetico.GeneracionProcesada += generacion =>
                    {
                        int progreso = generacion * tamañoBarra / parametros.LimiteGeneraciones;
                        string barra = new string('#', progreso).PadRight(tamañoBarra, '-');
                        Console.Write($"\r[{barra}] {generacion}/{parametros.LimiteGeneraciones}");
                    };
                }
                else
                {
                    algoritmoGenetico.GeneracionProcesada += generacion =>
                    {
                        Console.Write($"\rProcesando generación #{generacion}.");
                    };
                }

                using var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\rCancelación solicitada por el usuario.");
                    Console.ResetColor();
                    e.Cancel = true;
                    cts.Cancel();
                };

                var stopwatch = Stopwatch.StartNew();
                (Individuo mejorIndividuo, int generaciones) = algoritmoGenetico.Ejecutar(cts.Token);
                stopwatch.Stop();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nResultado encontrado después de {generaciones} generaciones.");
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
