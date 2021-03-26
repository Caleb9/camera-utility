using System;
using System.IO;

namespace CameraUtility.Commands
{
    internal abstract class ConsoleOutputBase
    {
        private readonly TextWriter _textWriter;

        protected internal ConsoleOutputBase(
            TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        protected void WriteLine(
            string? line = default,
            ConsoleColor? color = default)
        {
            PrivateWrite(line, _textWriter.WriteLine, color);
        }

        protected void Write(
            string line,
            ConsoleColor? color = default)
        {
            PrivateWrite(line, _textWriter.Write, color);
        }

        private static void PrivateWrite(
            string? line,
            Action<string?> writeMethod,
            ConsoleColor? color)
        {
            if (color.HasValue is false)
            {
                writeMethod(line);
                return;
            }
            var currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color!.Value;
                writeMethod(line);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }
    }
}