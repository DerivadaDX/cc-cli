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

            command.AddOption(instanciaOption);
            command.AddOption(maxGeneracionesOption);

            command.SetHandler((rutaInstancia, maxGeneraciones) =>
            {
                var parametros = new ParametrosSolucion
                {
                    RutaInstancia = rutaInstancia,
                    MaxGeneraciones = maxGeneraciones
                };

                var fileSystemHelper = FileSystemHelperFactory.Crear();
                var lector = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                Handler(parametros, lector);
            }, instanciaOption, maxGeneracionesOption);

            return command;
        }

        internal static void Handler(ParametrosSolucion parametros, LectorArchivoMatrizValoraciones lector)
        {
            decimal[,] matrizValoraciones = lector.Leer(parametros.RutaInstancia);
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);

            // TODO: crear población
            // TODO: crear algoritmo genético
            // TODO: ejecutar algoritmo genético
        }
    }
}
