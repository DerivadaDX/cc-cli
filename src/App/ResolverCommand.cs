using System.CommandLine;
using Common;
using Solver;

namespace App
{
    internal class ResolverCommand
    {
        internal static Command Create()
        {
            var command = new Command("resolver", "Resuelve una instancia");
            var instanciaOption = new Option<string>("--instancia") { IsRequired = true };
            var maxGeneracionesOption = new Option<int>("--max-generaciones", () => 0);
            var tamañoPoblacionOption = new Option<int>("--tamaño-poblacion", () => 100);

            command.AddOption(instanciaOption);
            command.AddOption(maxGeneracionesOption);
            command.AddOption(tamañoPoblacionOption);

            command.SetHandler((rutaInstancia, maxGeneraciones, tamañoPoblacion) =>
            {
                var parametros = new ParametrosSolucion(rutaInstancia, maxGeneraciones, tamañoPoblacion);

                var fileSystemHelper = FileSystemHelperFactory.Crear();
                var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                Handler(parametros, lector);
            }, instanciaOption, maxGeneracionesOption, tamañoPoblacionOption);

            return command;
        }

        internal static void Handler(ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector)
        {
            decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);
            var poblacion = new Poblacion(parametros.TamañoPoblacion);
            // TODO: instanciar algoritmo genético
            // TODO: ejecutar algoritmo genético
            // TODO: devoler resultado
        }
    }
}
