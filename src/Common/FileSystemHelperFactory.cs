namespace Common
{
    public class FileSystemHelperFactory
    {
        private static FileSystemHelper _helper = null;

        public static FileSystemHelper Crear()
        {
            var helper = _helper ?? new FileSystemHelper();
            return helper;
        }

#if DEBUG
        /// <summary>
        /// Solo usar para tests. Solamente está disponible en modo DEBUG.
        /// </summary>
        public static void SetearHelper(FileSystemHelper helper)
        {
            _helper = helper;
        }
#endif
    }
}
