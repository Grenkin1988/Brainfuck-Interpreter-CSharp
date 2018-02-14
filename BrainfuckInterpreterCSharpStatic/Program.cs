using System;

namespace BrainfuckInterpreterCSharpStatic {
    internal static class Program {
        private static void Main() { 
            string program = BrainfuckExamples.BrainfuckExamples.HelloWorld;
            BrainfuckInterpreter.Run(program);
            Console.ReadKey();
        }
    }
}
