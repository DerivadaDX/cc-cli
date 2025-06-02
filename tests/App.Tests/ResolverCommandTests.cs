using System.CommandLine;
using Common;
using NSubstitute;
using Solver;

namespace App.Tests
{
    public class ResolverCommandTests
    {
        private readonly Command _command;

        public ResolverCommandTests()
        {
            _command = ResolverCommand.Create();
        }

        [Fact]
        public void Create_NombreYOpciones_ConfiguradasCorrectamente()
        {
            Assert.Equal("resolver", _command.Name);
            Assert.Contains(_command.Options, o => o.Name == "instancia");
            Assert.Contains(_command.Options, o => o.Name == "max-generaciones");
            Assert.Contains(_command.Options, o => o.Name == "tamaño-poblacion");
        }

        [Fact]
        public void Create_MaxGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var maxGeneracionesOption = (Option<int>)_command.Options.First(o => o.Name == "max-generaciones");
            int maxGeneraciones = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(maxGeneracionesOption);

            Assert.Equal(0, maxGeneraciones);
        }

        [Fact]
        public void Create_TamañoPoblacionNoEspecificado_UsaValorPorDefecto()
        {
            var tamañoPoblacionOption = (Option<int>)_command.Options.First(o => o.Name == "tamaño-poblacion");
            int tamañoPoblacion = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(tamañoPoblacionOption);

            Assert.Equal(100, tamañoPoblacion);
        }

        [Fact]
        public void Handler_MatrizValoraciones_SeLee()
        {
            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            lector.Leer(Arg.Any<string>()).Returns(new decimal[,] { { 0, 3.9m }, { 1, 1.2m } });

            var parametros = new ParametrosSolucion("instancia.dat", 0, 2);
            ResolverCommand.Handler(parametros, lector);

            lector.Received(1).Leer(parametros.RutaInstancia);
        }
    }
}
