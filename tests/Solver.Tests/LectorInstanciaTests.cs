using Common;
using NSubstitute;

namespace Solver.Tests
{
    public class LectorInstanciaTests
    {
        [Fact]
        public void Constructor_FileSystemHelperNull_ArrojaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LectorInstancia(null));
            Assert.Contains("fileSystemHelper", ex.Message);
        }
    }
}
