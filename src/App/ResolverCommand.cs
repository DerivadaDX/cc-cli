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

                var consola = ConsoleProxyFactory.Crear();
                var presentador = new Presentador(consola);

                Handler(parametros, lector, presentador);

#if DEBUG
                Console.WriteLine("Presioná una tecla para salir...");
                Console.ReadKey();
#endif
            }, instanciaOption, limiteGeneracionesOption, cantidadIndividuosOption);

            return command;
        }

        internal static void Handler(ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector, Presentador presentador)
        {
            using var cts = new CancellationTokenSource();
            ConfigurarCancelacion(cts, presentador);

            try
            {
                var matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);
                ConfigurarProgreso(algoritmoGenetico, parametros, cts, presentador);

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
                presentador.MostrarAdvertencia("\nCancelación solicitada por el usuario.");
                e.Cancel = true;
                cts.Cancel();
            };
        }

        private static void ConfigurarProgreso(
            AlgoritmoGenetico algoritmoGenetico, ParametrosSolucion parametros, CancellationTokenSource cts, Presentador presentador)
        {
            if (parametros.LimiteGeneraciones > 0)
            {
                const int tamañoBarraProgreso = 50;
                algoritmoGenetico.GeneracionProcesada += generacion =>
                {
                    if (cts.IsCancellationRequested) return;

                    int progreso = generacion * tamañoBarraProgreso / parametros.LimiteGeneraciones;
                    string barraProgreso = new string('#', progreso).PadRight(tamañoBarraProgreso, '-');
                    string mensaje = $"[{barraProgreso}] {generacion}/{parametros.LimiteGeneraciones}";
                    presentador.MostrarProgreso(mensaje);
                };
            }
            else
            {
                algoritmoGenetico.GeneracionProcesada += generacion =>
                {
                    if (cts.IsCancellationRequested) return;

                    string mensaje = $"Procesando generación #{generacion}.";
                    presentador.MostrarProgreso(mensaje);
                };
            }
        }

        private static void MostrarResultado(Individuo mejorIndividuo, int generaciones, long tiempoMs, Presentador presentador)
        {
            presentador.MostrarExito($"\nResultado encontrado después de {generaciones} generaciones.");
            presentador.MostrarExito($"Resultado obtenido: {mejorIndividuo}.");
            presentador.MostrarExito($"Tiempo de ejecución: {tiempoMs} ms.");
        }

        private static void MostrarError(string mensaje, Presentador presentador)
        {
            presentador.MostrarError($"\nSe produjo un error: {mensaje}");
        }
    }
}
