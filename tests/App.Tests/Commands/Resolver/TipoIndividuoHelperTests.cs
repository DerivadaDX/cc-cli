using App.Commands.Resolver;
using Solver.Individuos;

namespace App.Tests.Commands.Resolver
{
    public class TipoIndividuoHelperTests
    {
        [Fact]
        public void Parse_ValorNoDefinido_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => TipoIndividuoHelper.Parse(null));
            Assert.Equal("valor", ex.ParamName);
        }

        [Fact]
        public void Parse_ValorIntercambio_DevuelveTipoIndividuoIntercambio()
        {
            TipoIndividuo resultado = TipoIndividuoHelper.Parse("intercambio");
            Assert.Equal(TipoIndividuo.IntercambioAsignaciones, resultado);
        }

        [Fact]
        public void Parse_ValorOptimizacion_DevuelveTipoIndividuoOptimizacion()
        {
            TipoIndividuo resultado = TipoIndividuoHelper.Parse("optimizacion");
            Assert.Equal(TipoIndividuo.OptimizacionAsignaciones, resultado);
        }

        [Fact]
        public void Parse_ValorMayusculas_DevuelveCorrespondienteTipoIndividuo()
        {
            TipoIndividuo resultado = TipoIndividuoHelper.Parse("INTERCAMBIO");
            Assert.Equal(TipoIndividuo.IntercambioAsignaciones, resultado);
        }

        [Fact]
        public void Parse_ValorMixtoMayusculasMinusculas_DevuelveCorrespondienteTipoIndividuo()
        {
            TipoIndividuo resultado = TipoIndividuoHelper.Parse("OptimizaCION");
            Assert.Equal(TipoIndividuo.OptimizacionAsignaciones, resultado);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("0")]
        [InlineData("xxx")]
        [InlineData("123")]
        [InlineData("invalidoption")]
        public void Parse_ValorInvalido_LanzaArgumentException(string valor)
        {
            var ex = Assert.Throws<ArgumentException>(() => TipoIndividuoHelper.Parse(valor));
            Assert.Contains("no reconocido", ex.Message);
            Assert.Equal("valor", ex.ParamName);
        }
    }
}