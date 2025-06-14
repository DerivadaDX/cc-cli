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
            }, instanciaOption, limiteGeneracionesOption, cantidadIndividuosOption);

            return command;
        }

        internal static void Handler(ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector)
        {
            decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
            var individuoFactory = new IndividuoIntercambioAsignacionesFactory(instanciaProblema);
            var poblacion = PoblacionFactory.Crear(parametros.CantidadIndividuos, individuoFactory);
            var algoritmoGenetico = new AlgoritmoGenetico(poblacion, parametros.LimiteGeneraciones);

            var stopwatch = Stopwatch.StartNew();
            (Individuo mejorIndividuo, int generaciones) = algoritmoGenetico.Ejecutar();
            stopwatch.Stop();

            Console.WriteLine($"Resultado encontrado después de {generaciones} generaciones.");
            Console.WriteLine($"Resultado obtendio: {mejorIndividuo}.");
            Console.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
