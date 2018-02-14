using System;
using System.IO;
using Xunit;

namespace BrainfuckInterpreterCSharp {
    public class BrainfuckInterpreterTest : IDisposable {
        private StringWriter _stringWriter;

        public BrainfuckInterpreterTest() {
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
        }

        [Fact]
        public void BrainfuckInterpreter_HelloWorld() {
            var interpreter = new BrainfuckInterpreter(BrainfuckExamples.BrainfuckExamples.HelloWorld);
            interpreter.Parse();
            interpreter.Run();

            Assert.Equal("Hello World!\n", _stringWriter.ToString());
        }

        [Fact]
        public void BrainfuckInterpreter_SimpleAddNumbersOutput() {
            var interpreter = new BrainfuckInterpreter(BrainfuckExamples.BrainfuckExamples.SimpleAddNumbersOutput);
            interpreter.Parse();
            interpreter.Run();

            Assert.Equal("7", _stringWriter.ToString());
        }

        public void Dispose() {
            _stringWriter?.Dispose();
        }
    }
}
