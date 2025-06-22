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
        private readonly Command _command;

        public GenerarCommandTests()
        {
            _command = GenerarCommand.Crear();
        }

        [Fact]
        public void Crear_NombreYOpciones_ConfiguradasCorrectamente()
        {
            Assert.Equal("generar", _command.Name);
            Assert.Contains(_command.Options, o => o.Name == "atomos");
            Assert.Contains(_command.Options, o => o.Name == "agentes");
            Assert.Contains(_command.Options, o => o.Name == "valor-maximo");
            Assert.Contains(_command.Options, o => o.Name == "output");
            Assert.Contains(_command.Options, o => o.Name == "disjuntas");
        }

        [Fact]
        public void Crear_ValorMaximoNoEspecificado_UsaValorPorDefecto()
        {
            var valorMaximoOption = (Option<int>)_command.Options.First(o => o.Name == "valor-maximo");
            int valorMaximo = _command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(valorMaximoOption);

            Assert.Equal(GenerarCommand.ValorMaximoPorDefecto, valorMaximo);
        }

        [Fact]
        public void Crear_OutputNoEspecificada_UsaValorPorDefecto()
        {
            var outputOption = (Option<string>)_command.Options.First(o => o.Name == "output");
            string output = _command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(outputOption);

            Assert.Equal(GenerarCommand.RutaSalidaPorDefecto, output);
        }

        [Fact]
        public void Crear_DisjuntasNoEspecificada_UsaFalse()
        {
            var disjuntasOption = (Option<bool>)_command.Options.First(o => o.Name == "disjuntas");
            bool disjuntas = _command.Parse("generar --atomos 5 --agentes 3").GetValueForOption(disjuntasOption);

            Assert.False(disjuntas);
        }

        [Fact]
        public void EjecutarGeneracion_ConParametrosValidos_GeneraYEscribeInstancia()
        {
            var instanciaMock = new decimal[1, 1];

            var builder = Substitute.For<InstanciaBuilder>(Substitute.For<GeneradorNumerosRandom>());
            builder.ConCantidadDeAtomos(Arg.Any<int>()).Returns(builder);
            builder.ConCantidadDeAgentes(Arg.Any<int>()).Returns(builder);
            builder.ConValorMaximo(Arg.Any<int>()).Returns(builder);
            builder.ConValoracionesDisjuntas(Arg.Any<bool>()).Returns(builder);
            builder.Build().Returns(instanciaMock);

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
            escritor.Received(1).EscribirInstancia(instanciaMock, "instancia.dat");
        }
    }
}