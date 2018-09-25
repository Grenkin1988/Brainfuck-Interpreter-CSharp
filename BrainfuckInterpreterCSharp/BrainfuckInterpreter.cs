using System.Collections.Generic;

namespace BrainfuckInterpreterCSharp {
    public class ProgramData {
        public byte[] ByteCells { get; set; } = new byte[5];
        public uint Pointer { get; set; }
    }

    public class BrainfuckInterpreter {
        private readonly string _inputProgram;
        private ProgramData _currentData = new ProgramData();
        private readonly List<ICommand> _commands = new List<ICommand>();
        private int? _currentLoopIndex;

        public BrainfuckInterpreter(string program) {
            _inputProgram = program;
        }

        public void Parse() {
            foreach (char nextChar in _inputProgram) {
                switch (nextChar) {
                    case '>': {
                        AddNextCommand(new MovePointerToRightCommand());
                        break;
                    }
                    case '<': {
                        AddNextCommand(new MovePointerToLeftCommand());
                        break;
                    }
                    case '+': {
                        AddNextCommand(new IncreaseCurrentCellCommand());
                        break;
                    }
                    case '-': {
                        AddNextCommand(new DecreaseCurrentCellCommand());
                        break;
                    }
                    case '.': {
                        AddNextCommand(new PrintCurrentCellCommand());
                        break;
                    }
                    case ',': {
                        AddNextCommand(new ReadToCurrentCellCommand());
                        break;
                    }
                    case '[': {
                        AddNextCommand(new LoopCommand());
                        break;
                    }
                    case ']': {
                        EndCurrentLoop();
                        break;
                    }
                }
            }
        }

        private void AddNextCommand(ICommand command) {
            if (_currentLoopIndex.HasValue
                && _commands[_currentLoopIndex.Value] is LoopCommand loop) {
                loop.AddCommand(command);
                return;
            }

            _commands.Add(command);
            if (command is LoopCommand) {
                _currentLoopIndex = _commands.Count - 1;
            }
        }

        private void EndCurrentLoop() {
            if (_currentLoopIndex.HasValue
                && _commands[_currentLoopIndex.Value] is LoopCommand loop
                && loop.HasOpenLoop()) {                
                loop.EndCurrentLoop();
            } else {
                _currentLoopIndex = null;
            }
        }

        public void Run() {
            foreach (ICommand command in _commands) {
                _currentData = command.Execute(_currentData);
            }
        }
    }
}
