using System.CommandLine;
using System.CommandLine.Parsing;
using App.Commands.Generar;
using Common;
using Generator;
using NSubstitute;

namespace App.Tests.Commands.Generar
{
    public class GenerarCommandTests
    {
        [Fact]
        public void Crear_NombreYOpciones_ConfiguradasCorrectamente()
        {
            var comandoGenerar = GenerarCommand.Crear();

            Assert.Equal("generar", comandoGenerar.Name);
            Assert.Contains(comandoGenerar.Arguments, a => a.Name == "ruta-salida");
            Assert.Contains(comandoGenerar.Options, o => o.Name == "atomos");
            Assert.Contains(comandoGenerar.Options, o => o.Name == "agentes");
            Assert.Contains(comandoGenerar.Options, o => o.Name == "valor-maximo");
            Assert.Contains(comandoGenerar.Options, o => o.Name == "disjuntas");
        }

        [Fact]
        public void Crear_RutaSalidaNoEspecificada_UsaValorPorDefecto()
        {
            var comandoGenerar = GenerarCommand.Crear();
            var rutaSalidaArgument = (Argument<string>)comandoGenerar.Arguments.First(a => a.Name == "ruta-salida");

            string rutaSalida = comandoGenerar.Parse("--atomos 5 --agentes 3").GetValueForArgument(rutaSalidaArgument);

            Assert.Equal(GenerarCommand.RutaSalidaPorDefecto, rutaSalida);
        }

        [Fact]
        public void Crear_AtomosNoEspecificado_IndicaQueEsCampoRequerido()
        {
            var comandoGenerar = GenerarCommand.Crear();

            ParseResult resultadoParseo = comandoGenerar.Parse("--agentes 3");

            Assert.Single(resultadoParseo.Errors);
            Assert.Contains(resultadoParseo.Errors, e => e.Message.Contains("--atomos"));
        }

        [Fact]
        public void Crear_AgentesNoEspecificado_IndicaQueEsCampoRequerido()
        {
            var comandoGenerar = GenerarCommand.Crear();

            ParseResult resultadoParseo = comandoGenerar.Parse("--atomos 5");

            Assert.Single(resultadoParseo.Errors);
            Assert.Contains(resultadoParseo.Errors, e => e.Message.Contains("--agentes"));
        }

        [Fact]
        public void Crear_ValorMaximoNoEspecificado_UsaValorPorDefecto()
        {
            var comandoGenerar = GenerarCommand.Crear();
            var valorMaximoOption = (Option<int>)comandoGenerar.Options.First(o => o.Name == "valor-maximo");

            int valorMaximo = comandoGenerar.Parse("--atomos 5 --agentes 3").GetValueForOption(valorMaximoOption);

            Assert.Equal(GenerarCommand.ValorMaximoPorDefecto, valorMaximo);
        }

        [Fact]
        public void Crear_DisjuntasNoEspecificada_UsaFalse()
        {
            var comandoGenerar = GenerarCommand.Crear();
            var disjuntasOption = (Option<bool>)comandoGenerar.Options.First(o => o.Name == "disjuntas");

            bool disjuntas = comandoGenerar.Parse("--atomos 5 --agentes 3").GetValueForOption(disjuntasOption);

            Assert.False(disjuntas);
        }

        [Fact]
        public void EjecutarGeneracion_ConParametrosValidos_GeneraYEscribeInstancia()
        {
            var instanciaConstruida = new decimal[1, 1];

            var builder = Substitute.For<InstanciaBuilder>(Substitute.For<GeneradorNumerosRandom>());
            builder.ConCantidadDeAtomos(Arg.Any<int>()).Returns(builder);
            builder.ConCantidadDeAgentes(Arg.Any<int>()).Returns(builder);
            builder.ConValorMaximo(Arg.Any<int>()).Returns(builder);
            builder.ConValoracionesDisjuntas(Arg.Any<bool>()).Returns(builder);
            builder.Build().Returns(instanciaConstruida);

            var parametros = new ParametrosGeneracion
            {
                Atomos = 5,
                Agentes = 3,
                ValorMaximo = 100,
                RutaSalida = "instancia.dat",
                ValoracionesDisjuntas = true,
            };
            var escritor = Substitute.For<EscritorInstancia>(Substitute.For<FileSystemHelper>());
            var presentador = Substitute.For<Presentador>(Substitute.For<ConsoleProxy>());

            GenerarCommand.EjecutarGeneracion(parametros, builder, escritor, presentador);

            builder.Received(1).ConCantidadDeAtomos(5);
            builder.Received(1).ConCantidadDeAgentes(3);
            builder.Received(1).ConValorMaximo(100);
            builder.Received(1).ConValoracionesDisjuntas(true);
            builder.Received(1).Build();
            escritor.Received(1).EscribirInstancia(instanciaConstruida, "instancia.dat");
        }
    }
}