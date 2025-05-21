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
                var fileSystemHelper = FileSystemHelperFactory.Crear();
                var lectorMatrizValoraciones = new LectorArchivoMatrizValoraciones(fileSystemHelper);

                Handler(lectorMatrizValoraciones, rutaInstancia, maxGeneraciones);
            }, instanciaOption, maxGeneracionesOption);

            return command;
        }

        internal static void Handler(LectorArchivoMatrizValoraciones lector, string rutaInstancia, int maxGeneraciones)
        {
            decimal[,] matrizValoraciones = lector.Leer(rutaInstancia);
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matrizValoraciones);

            // TODO: crear población
            // TODO: crear algoritmo genético
            // TODO: ejecutar algoritmo genético
        }
    }
}
