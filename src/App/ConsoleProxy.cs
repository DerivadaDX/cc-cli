


namespace App
{
    internal class ConsoleProxy
    {
        internal virtual void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        internal virtual void ForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        internal virtual void ResetColor()
        {
            Console.ResetColor();
        }
    }
}
