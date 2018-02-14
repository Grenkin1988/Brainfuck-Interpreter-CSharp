using System;

namespace BrainfuckInterpreterCSharp {
    internal static class Program {
        private static void Main(string[] args) {
            
            var interpreter = new BrainfuckInterpreter(BrainfuckExamples.BrainfuckExamples.HelloWorld);
            interpreter.Parse();
            interpreter.Run();

            Console.ReadKey();
        }
    }
}
