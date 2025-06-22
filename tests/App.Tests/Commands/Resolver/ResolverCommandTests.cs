using System.CommandLine;
using App.Commands.Resolver;
using Common;
using NSubstitute;
using Solver;

namespace App.Tests.Commands.Resolver
{
    public class ResolverCommandTests
    {
        [Fact]
        public void Crear_NombreYOpciones_ConfiguradasCorrectamente()
        {
            var comandoResolver = ResolverCommand.Crear();

            Assert.Equal("resolver", comandoResolver.Name);
            Assert.Contains(comandoResolver.Options, o => o.Name == "instancia");
            Assert.Contains(comandoResolver.Options, o => o.Name == "limite-generaciones");
            Assert.Contains(comandoResolver.Options, o => o.Name == "cantidad-individuos");
            Assert.Contains(comandoResolver.Options, o => o.Name == "limite-estancamiento");
            Assert.Contains(comandoResolver.Options, o => o.Name == "tipo-individuo");
        }

        [Fact]
        public void Crear_LimiteGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var limiteGeneracionesOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "limite-generaciones");

            int limiteGeneraciones = comandoResolver
                .Parse("resolver --instancia instancia.dat")
                .GetValueForOption(limiteGeneracionesOption);

            Assert.Equal(0, limiteGeneraciones);
        }

        [Fact]
        public void Crear_CantidadIndividuosNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var cantidadIndividuosOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "cantidad-individuos");

            int cantidadIndividuos = comandoResolver
                .Parse("resolver --instancia instancia.dat")
                .GetValueForOption(cantidadIndividuosOption);

            Assert.Equal(100, cantidadIndividuos);
        }

        [Fact]
        public void Crear_LimiteEstancamientoNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var limiteEstancamientoOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "limite-estancamiento");

            int limiteEstancamiento = comandoResolver
                .Parse("resolver --instancia instancia.dat")
                .GetValueForOption(limiteEstancamientoOption);

            Assert.Equal(1000, limiteEstancamiento);
        }

        [Fact]
        public void Crear_TipoIndividuoNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var tipoIndividuoOption = (Option<string>)comandoResolver.Options.First(o => o.Name == "tipo-individuo");

            string tipoIndividuo = comandoResolver
                .Parse("resolver --instancia instancia.dat")
                .GetValueForOption(tipoIndividuoOption);

            Assert.Equal("intercambio", tipoIndividuo);
        }

        [Fact]
        public void EjecutarResolucion_MatrizValoraciones_SeLee()
        {
            var parametros = new ParametrosSolucion
            {
                RutaInstancia = "ruta/a/instancia.dat",
            };

            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            var presentador = Substitute.For<Presentador>(Substitute.For<ConsoleProxy>());

            ResolverCommand.EjecutarResolucion(parametros, lector, presentador);

            lector.Received(1).Leer("ruta/a/instancia.dat");
        }
    }
}
