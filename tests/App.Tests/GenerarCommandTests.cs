using System.CommandLine;
using System.CommandLine.Parsing;
using Common;
using GeneradorInstancia;
using NSubstitute;

namespace App.Tests
{
    public class GenerarCommandTests
    {
        [Fact]
        public void Create_ValorMaximoNoEspecificado_UsaValorPorDefecto()
        {
            var command = GenerarCommand.Create();
            var valorMaximoOption = (Option<int>)command.Options.First(o => o.Name == "valor-maximo");
            int valorMaximo = command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(valorMaximoOption);

            Assert.Equal(GenerarCommand.ValorMaximoPorDefecto, valorMaximo);
        }

        [Fact]
        public void Create_OutputNoEspecificada_UsaValorPorDefecto()
        {
            var command = GenerarCommand.Create();
            var outputOption = (Option<string>)command.Options.First(o => o.Name == "output");
            string output = command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(outputOption);

            Assert.Equal(GenerarCommand.RutaSalidaPorDefecto, output);
        }

        [Fact]
        public void Create_DisjuntasNoEspecificada_UsaFalse()
        {
            var command = GenerarCommand.Create();
            var disjuntasOption = (Option<bool>)command.Options.First(o => o.Name == "disjuntas");
            bool disjuntas = command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(disjuntasOption);

            Assert.False(disjuntas);
        }

        [Fact]
        public void Create_Command_ConfiguraOpcionesCorrectamente()
        {
            var command = GenerarCommand.Create();

            Assert.Equal("generar", command.Name);
            Assert.Contains(command.Options, o => o.Name == "atomos");
            Assert.Contains(command.Options, o => o.Name == "agentes");
            Assert.Contains(command.Options, o => o.Name == "valor-maximo");
            Assert.Contains(command.Options, o => o.Name == "output");
            Assert.Contains(command.Options, o => o.Name == "disjuntas");
        }

        [Fact]
        public void Handler_ConParametrosValidos_GeneraYEscribeInstancia()
        {
            var instanciaMock = new decimal[1, 1];

            var builder = Substitute.For<InstanciaBuilder>();
            builder.ConCantidadDeAtomos(Arg.Any<int>()).Returns(builder);
            builder.ConCantidadDeJugadores(Arg.Any<int>()).Returns(builder);
            builder.ConValorMaximo(Arg.Any<int>()).Returns(builder);
            builder.ConValoracionesDisjuntas(Arg.Any<bool>()).Returns(builder);
            builder.Build().Returns(instanciaMock);

            var escritor = Substitute.For<EscritorInstancia>(Substitute.For<FileSystemHelper>());

            var parametros = new ParametrosGeneracion(
                atomos: 5,
                agentes: 3,
                valorMaximo: 100,
                rutaSalida: "instancia.dat",
                valoracionesDisjuntas: true);

            GenerarCommand.Handler(parametros, builder, escritor);

            builder.Received(1).ConCantidadDeAtomos(5);
            builder.Received(1).ConCantidadDeJugadores(3);
            builder.Received(1).ConValorMaximo(100);
            builder.Received(1).ConValoracionesDisjuntas(true);
            builder.Received(1).Build();
            escritor.Received(1).EscribirInstancia(instanciaMock, "instancia.dat");
        }
    }
}