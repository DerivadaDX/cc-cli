using System.CommandLine;

namespace App
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Herramienta CLI para Generación/Resolución de Instancias de Cake Cutting");
            rootCommand.SetHandler(() => Console.WriteLine("Use un subcomando (generar/resolver). Ver ayuda con --help."));
            rootCommand.AddCommand(GenerarCommand.Create());
            rootCommand.AddCommand(ResolverCommand.Create());

            if (args.Length == 0)
            {
                Console.WriteLine("Use un subcomando (generar/resolver). Ver ayuda con --help.");
                return await rootCommand.InvokeAsync("--help");
            }

            int exitCode = await rootCommand.InvokeAsync(args);
            return exitCode;
        }
    }
}