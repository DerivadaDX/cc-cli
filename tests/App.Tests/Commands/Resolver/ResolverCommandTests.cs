using System.CommandLine;
using App.Commands.Resolver;
using Common;
using NSubstitute;
using Solver;

namespace App.Tests.Commands.Resolver
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
            Assert.Contains(_command.Options, o => o.Name == "limite-generaciones");
            Assert.Contains(_command.Options, o => o.Name == "cantidad-individuos");
        }

        [Fact]
        public void Create_LimiteGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var limiteGeneracionesOption = (Option<int>)_command.Options.First(o => o.Name == "limite-generaciones");
            int limiteGeneraciones = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(limiteGeneracionesOption);

            Assert.Equal(0, limiteGeneraciones);
        }

        [Fact]
        public void Create_CantidadIndividuosNoEspecificado_UsaValorPorDefecto()
        {
            var cantidadIndividuosOption = (Option<int>)_command.Options.First(o => o.Name == "cantidad-individuos");
            int cantidadIndividuos = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(cantidadIndividuosOption);

            Assert.Equal(100, cantidadIndividuos);
        }

        [Fact]
        public void Handler_MatrizValoraciones_SeLee()
        {
            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            lector.Leer(Arg.Any<string>()).Returns(new decimal[,] { { 0, 3.9m }, { 1, 1.2m } });

            var parametros = new ParametrosSolucion("instancia.dat", 1, 2);
            ResolverCommand.Handler(parametros, lector, Substitute.For<Presentador>(Substitute.For<ConsoleProxy>()));

            lector.Received(1).Leer(parametros.RutaInstancia);
        }
    }
}
