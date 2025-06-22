namespace Common.Tests
{
    public class FileSystemHelperFactoryTests : IDisposable
    {
        public void Dispose()
        {
            FileSystemHelperFactory.SetearHelper(null);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Crear_InstanciaDevuelta_EsValida()
        {
            var helper = FileSystemHelperFactory.Crear();
            Assert.NotNull(helper);
            Assert.IsType<FileSystemHelper>(helper);
        }

        [Fact]
        public void SetearHelper_Helper_SeSeteaCorrectamente()
        {
            var helperSeteado = new FileSystemHelper();
            FileSystemHelperFactory.SetearHelper(helperSeteado);

            var helperObtenido = FileSystemHelperFactory.Crear();

            Assert.Same(helperSeteado, helperObtenido);
        }
    }
}
