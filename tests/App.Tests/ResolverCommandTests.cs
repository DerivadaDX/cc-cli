using System.CommandLine;

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
        }

        [Fact]
        public void Create_MaxGeneracionesNoEspecificado_UsaValorPorDefecto()
        {
            var maxGeneracionesOption = (Option<int>)_command.Options.First(o => o.Name == "max-generaciones");
            int maxGeneraciones = _command.Parse("resolver --instancia instancia.dat").GetValueForOption(maxGeneracionesOption);

            Assert.Equal(0, maxGeneraciones);
        }
    }
}
