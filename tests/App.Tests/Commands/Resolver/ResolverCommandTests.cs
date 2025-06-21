using System.CommandLine;
using App.Commands.Resolver;
using Common;
using NSubstitute;
using Solver;
using Solver.Individuos;

namespace App.Tests.Commands.Resolver
{
    public class ResolverCommandTests
    {
        private readonly Command _command;

        public ResolverCommandTests()
        {
            _command = ResolverCommand.Crear();
        }

        [Fact]
        public void Crear_NombreYOpciones_ConfiguradasCorrectamente()
        {
            Assert.Equal("resolver", _command.Name);
            Assert.Contains(_command.Options, o => o.Name == "instancia");
            Assert.Contains(_command.Options, o => o.Name == "limite-generaciones");
            Assert.Contains(_command.Options, o => o.Name == "cantidad-individuos");
            Assert.Contains(_command.Options, o => o.Name == "limite-estancamiento");
            Assert.Contains(_command.Options, o => o.Name == "tipo-individuo");
        }

        [Fact]
        public void Crear_LimiteGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var limiteGeneracionesOption = (Option<int>)_command.Options.First(o => o.Name == "limite-generaciones");
            int limiteGeneraciones = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(limiteGeneracionesOption);

            Assert.Equal(0, limiteGeneraciones);
        }

        [Fact]
        public void Crear_CantidadIndividuosNoEspecificado_UsaValorPorDefecto()
        {
            var cantidadIndividuosOption = (Option<int>)_command.Options.First(o => o.Name == "cantidad-individuos");
            int cantidadIndividuos = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(cantidadIndividuosOption);

            Assert.Equal(100, cantidadIndividuos);
        }

        [Fact]
        public void Crear_LimiteEstancamientoNoEspecificado_UsaValorPorDefecto()
        {
            var limiteEstancamientoOption = (Option<int>)_command.Options.First(o => o.Name == "limite-estancamiento");
            int limiteEstancamiento = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(limiteEstancamientoOption);

            Assert.Equal(1000, limiteEstancamiento);
        }

        [Fact]
        public void Crear_TipoIndividuoNoEspecificado_UsaValorPorDefecto()
        {
            var tipoIndividuoOption = (Option<string>)_command.Options.First(o => o.Name == "tipo-individuo");
            string tipoIndividuo = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(tipoIndividuoOption);

            Assert.Equal("intercambio", tipoIndividuo);
        }

        [Fact]
        public void EjecutarResolucion_MatrizValoraciones_SeLee()
        {
            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            lector.Leer(Arg.Any<string>()).Returns(new decimal[,] { { 0, 3.9m }, { 1, 1.2m } });

            var parametros = new ParametrosSolucion("instancia.dat", 1, 2, 3, TipoIndividuo.Intercambio);
            ResolverCommand.EjecutarResolucion(parametros, lector, Substitute.For<Presentador>(Substitute.For<ConsoleProxy>()));

            lector.Received(1).Leer(parametros.RutaInstancia);
        }
    }
}
