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
            var helperMock = new FileSystemHelper();
            FileSystemHelperFactory.SetearHelper(helperMock);

            var helper = FileSystemHelperFactory.Crear();

            Assert.Same(helperMock, helper);
        }
    }
}
