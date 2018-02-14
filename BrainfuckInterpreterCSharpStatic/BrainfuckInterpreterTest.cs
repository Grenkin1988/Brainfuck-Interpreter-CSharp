using System;
using System.IO;
using Xunit;

namespace BrainfuckInterpreterCSharpStatic {
    public class BrainfuckInterpreterTest : IDisposable {
        private StringWriter _stringWriter;

        public BrainfuckInterpreterTest() {
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
        }

        [Fact]
        public void BrainfuckInterpreter_HelloWorld() {
            BrainfuckInterpreter.Run(BrainfuckExamples.BrainfuckExamples.HelloWorld);

            Assert.Equal("Hello World!\n", _stringWriter.ToString());
        }

        [Fact]
        public void BrainfuckInterpreter_SimpleAddNumbersOutput() {
            BrainfuckInterpreter.Run(BrainfuckExamples.BrainfuckExamples.SimpleAddNumbersOutput);

            Assert.Equal("7", _stringWriter.ToString());
        }

        public void Dispose() {
            _stringWriter?.Dispose();
        }
    }
}
