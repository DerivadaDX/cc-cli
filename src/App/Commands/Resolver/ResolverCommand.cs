using System.CommandLine;
using System.Diagnostics;
using Common;
using Solver;
using Solver.Individuos;

namespace App.Commands.Resolver
{
    internal class ResolverCommand
    {
        private readonly Presentador _presentador;

        internal ResolverCommand(Presentador presentador)
        {
            ArgumentNullException.ThrowIfNull(presentador, nameof(presentador));
            _presentador = presentador;
        }

        internal Command Crear()
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
            command.SetHandler(EjecutarResolucion, instanciaOption, limiteGeneracionesOption, cantidadIndividuosOption);

            return command;
        }

        private void EjecutarResolucion(string rutaInstancia, int limiteGeneraciones, int cantidadIndividuos)
        {
            var parametros = new ParametrosSolucion(rutaInstancia, limiteGeneraciones, cantidadIndividuos);

            var fileSystemHelper = FileSystemHelperFactory.Crear();
            var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);
            using var cts = new CancellationTokenSource();
            ConfigurarCancelacion(cts);

            try
            {
                var matrizValoraciones = lector.Leer(parametros.RutaInstancia);
                var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
                var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
                var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);

                var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);
                ConfigurarProgreso(parametros, algoritmoGenetico);

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

        private void ConfigurarCancelacion(CancellationTokenSource cts)
        {
            Console.CancelKeyPress += (_, e) =>
            {
                _presentador.MostrarAdvertencia("\nCancelación solicitada por el usuario.");
                e.Cancel = true;
                cts.Cancel();
            };
        }

        private void ConfigurarProgreso(ParametrosSolucion parametros, AlgoritmoGenetico algoritmoGenetico)
        {
            if (parametros.LimiteGeneraciones > 0)
            {
                const int tamañoBarraProgreso = 50;
                algoritmoGenetico.GeneracionProcesada += (generacion, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    int progreso = generacion * tamañoBarraProgreso / parametros.LimiteGeneraciones;
                    string barraProgreso = new string('#', progreso).PadRight(tamañoBarraProgreso, '-');
                    string mensaje = $"[{barraProgreso}] {generacion}/{parametros.LimiteGeneraciones}";
                    _presentador.MostrarProgreso(mensaje);
                };
            }
            else
            {
                algoritmoGenetico.GeneracionProcesada += (generacion, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    string mensaje = $"Procesando generación #{generacion}.";
                    _presentador.MostrarProgreso(mensaje);
                };
            }
        }

        private void MostrarResultado(Individuo mejorIndividuo, int generaciones, long tiempoMs)
        {
            _presentador.MostrarExito($"\nResultado encontrado después de {generaciones} generaciones.");
            _presentador.MostrarExito($"Resultado obtenido: {mejorIndividuo}.");
            _presentador.MostrarExito($"Tiempo de ejecución: {tiempoMs} ms.");
        }

        private void MostrarError(string mensaje)
        {
            _presentador.MostrarError($"\nSe produjo un error: {mensaje}");
        }
    }
}
