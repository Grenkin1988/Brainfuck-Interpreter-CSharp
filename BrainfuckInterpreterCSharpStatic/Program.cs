namespace BrainfuckInterpreterCSharpStatic {
    internal static class Program {
        private static void Main(string[] args) { 
            string program = BrainfuckExamples.BrainfuckExamples.HelloWorld;
            BrainfuckInterpreter.Run(program);
        }
    }
}
