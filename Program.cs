using System;

namespace BrainfuckInterpreterCSharp {
    internal static class Program {
        private static void Main(string[] args) {
            
            var interpreter = new BrainfuckInterpreter(BrainfuckExamples.ROT13);
            interpreter.Parse();
            interpreter.Run();

            Console.ReadKey();
        }
    }
}
