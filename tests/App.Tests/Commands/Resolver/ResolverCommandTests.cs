using System.CommandLine;
using System.CommandLine.Parsing;
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
            Assert.Contains(comandoResolver.Arguments, a => a.Name == "ruta-instancia");
            Assert.Contains(comandoResolver.Options, o => o.Name == "limite-generaciones");
            Assert.Contains(comandoResolver.Options, o => o.Name == "cantidad-individuos");
            Assert.Contains(comandoResolver.Options, o => o.Name == "limite-estancamiento");
            Assert.Contains(comandoResolver.Options, o => o.Name == "tipo-individuo");
            Assert.Contains(comandoResolver.Options, o => o.Name == "seed");
        }

        [Fact]
        public void Crear_RutaInstanciaNoEspecificada_IndicaQueEsCampoRequerido()
        {
            var comandoResolver = ResolverCommand.Crear();

            ParseResult resultadoParseo = comandoResolver.Parse("");

            Assert.Single(resultadoParseo.Errors);
        }

        [Fact]
        public void Crear_LimiteGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var limiteGeneracionesOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "limite-generaciones");

            int limiteGeneraciones = comandoResolver.Parse("instancia.dat").GetValueForOption(limiteGeneracionesOption);

            Assert.Equal(0, limiteGeneraciones);
        }

        [Fact]
        public void Crear_CantidadIndividuosNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var cantidadIndividuosOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "cantidad-individuos");

            int cantidadIndividuos = comandoResolver.Parse("instancia.dat").GetValueForOption(cantidadIndividuosOption);

            Assert.Equal(100, cantidadIndividuos);
        }

        [Fact]
        public void Crear_LimiteEstancamientoNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var limiteEstancamientoOption = (Option<int>)comandoResolver.Options.First(o => o.Name == "limite-estancamiento");

            int limiteEstancamiento = comandoResolver.Parse("instancia.dat").GetValueForOption(limiteEstancamientoOption);

            Assert.Equal(1000, limiteEstancamiento);
        }

        [Fact]
        public void Crear_TipoIndividuoNoEspecificado_UsaValorPorDefecto()
        {
            var comandoResolver = ResolverCommand.Crear();
            var tipoIndividuoOption = (Option<string>)comandoResolver.Options.First(o => o.Name == "tipo-individuo");

            string tipoIndividuo = comandoResolver.Parse("instancia.dat").GetValueForOption(tipoIndividuoOption);

            Assert.Equal("optimizacion", tipoIndividuo);
        }

        [Fact]
        public void Crear_SemillaNoEspecificada_UsaNull()
        {
            var comandoResolver = ResolverCommand.Crear();
            var seedOption = (Option<int?>)comandoResolver.Options.First(o => o.Name == "seed");

            int? seed = comandoResolver.Parse("instancia.dat").GetValueForOption(seedOption);

            Assert.Null(seed);
        }

        [Fact]
        public void EjecutarResolucion_ValorDeSeed_SePresenta()
        {
            var parametros = new ParametrosSolucion { RutaInstancia = "ruta/a/instancia.dat", Seed = 123 };

            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            var presentador = Substitute.For<Presentador>(Substitute.For<ConsoleProxy>());
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            ResolverCommand.EjecutarResolucion(parametros, lector, presentador, generadorRandom);

            presentador.Received(1).MostrarInfo("Seed utilizada: 123");
        }

        [Fact]
        public void EjecutarResolucion_MatrizValoraciones_SeLee()
        {
            var parametros = new ParametrosSolucion { RutaInstancia = "ruta/a/instancia.dat" };

            var lector = Substitute.For<LectorArchivoMatrizValoraciones>(Substitute.For<FileSystemHelper>());
            var presentador = Substitute.For<Presentador>(Substitute.For<ConsoleProxy>());
            var generadorRandom = Substitute.For<GeneradorNumerosRandom>(1);

            ResolverCommand.EjecutarResolucion(parametros, lector, presentador, generadorRandom);

            lector.Received(1).Leer("ruta/a/instancia.dat");
        }
    }
}
