using System.CommandLine;
using System.Text;
using App.Commands.Generar;
using App.Commands.Resolver;

namespace App
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var rootCommand = new RootCommand("Herramienta CLI para Generación/Resolución de instancias de Cake Cutting");
            rootCommand.AddCommand(GenerarCommand.Create());
            rootCommand.AddCommand(ResolverCommand.Create());

            if (args.Length == 0)
            {
                Console.WriteLine("Use un subcomando (generar/resolver). Ver ayuda con --help.");
                Console.WriteLine();
                return await rootCommand.InvokeAsync("--help");
            }

            int exitCode = await rootCommand.InvokeAsync(args);
            return exitCode;
        }
    }
}