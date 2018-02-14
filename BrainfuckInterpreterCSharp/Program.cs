using System;

namespace BrainfuckInterpreterCSharp {
    internal static class Program {
        private static void Main() {
            
            var interpreter = new BrainfuckInterpreter(BrainfuckExamples.BrainfuckExamples.HelloWorld);
            interpreter.Parse();
            interpreter.Run();

            Console.ReadKey();
        }
    }
}
