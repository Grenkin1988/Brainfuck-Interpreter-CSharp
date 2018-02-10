using System;
using System.Collections.Generic;

namespace BrainfuckInterpreterCSharp {
    public interface ICommand {
        ProgrammData Execute(ProgrammData data);
    }

    public class MovePointerToRightCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            data.Pointer++;
            byte[] byteCells = data.ByteCells;
            if (data.Pointer >= data.ByteCells.Length) {
                byteCells = AddCell(data.ByteCells);
            }

            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = byteCells
            };
        }

        private byte[] AddCell(byte[] byteCells) {
            var newByteCells = new byte[byteCells.Length + 1];
            for (int i = 0; i < byteCells.Length; i++) {
                newByteCells[i] = byteCells[i];
            }

            return newByteCells;
        }

        public override string ToString() => ">";
    }

    public class MovePointerToLeftCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            if (data.Pointer == 0) {
                return data;
            }

            data.Pointer--;
            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "<";
    }

    public class IncreaseCurrentCellCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            data.ByteCells[data.Pointer]++;
            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "+";
    }

    public class DecreaseCurrentCellCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            data.ByteCells[data.Pointer]--;
            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "-";
    }

    public class PrintCurrentCellCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            Console.Write((char)data.ByteCells[data.Pointer]);
            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => ".";
    }

    public class ReadToCurrentCellCommand : ICommand {
        public ProgrammData Execute(ProgrammData data) {
            ConsoleKeyInfo key = Console.ReadKey();
            data.ByteCells[data.Pointer] = (byte)key.KeyChar;
            return new ProgrammData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => ",";
    }

    public class LoopCommand : ICommand {
        private List<ICommand> _commands = new List<ICommand>();
        private ProgrammData _currentData;
        private int? _currentLoopIndex;

        public ProgrammData Execute(ProgrammData data) {
            _currentData = data;

            while (_currentData.ByteCells[_currentData.Pointer] != 0) {
                foreach (ICommand command in _commands) {
                    _currentData = command.Execute(_currentData);
                }
            }

            return _currentData;
        }

        public void AddCommand(ICommand command) {
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

        public bool HasOpenLoop() {
            return _currentLoopIndex.HasValue
                && _commands[_currentLoopIndex.Value] is LoopCommand;
        }

        public void EndCurrentLoop() {
            if (_currentLoopIndex.HasValue
                && _commands[_currentLoopIndex.Value] is LoopCommand loop
                && loop.HasOpenLoop()) {
                loop.EndCurrentLoop();
            } else {
                _currentLoopIndex = null;
            }
        }

        public override string ToString() {
            string result = "[";

            foreach (ICommand command in _commands) {
                result += command;
            }

            result += "]";
            return result;
        }
    }
}
