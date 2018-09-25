using System;
using System.Collections.Generic;

namespace BrainfuckInterpreterCSharp {
    public interface ICommand {
        ProgramData Execute(ProgramData data);
    }

    public class MovePointerToRightCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            data.Pointer++;
            byte[] byteCells = data.ByteCells;
            if (data.Pointer >= data.ByteCells.Length) {
                byteCells = AddCell(data.ByteCells);
            }

            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = byteCells
            };
        }

        private static byte[] AddCell(IReadOnlyList<byte> byteCells) {
            var newByteCells = new byte[byteCells.Count + 1];
            for (int i = 0; i < byteCells.Count; i++) {
                newByteCells[i] = byteCells[i];
            }

            return newByteCells;
        }

        public override string ToString() => ">";
    }

    public class MovePointerToLeftCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            if (data.Pointer == 0) {
                return data;
            }

            data.Pointer--;
            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "<";
    }

    public class IncreaseCurrentCellCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            data.ByteCells[data.Pointer]++;
            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "+";
    }

    public class DecreaseCurrentCellCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            data.ByteCells[data.Pointer]--;
            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => "-";
    }

    public class PrintCurrentCellCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            Console.Write((char)data.ByteCells[data.Pointer]);
            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => ".";
    }

    public class ReadToCurrentCellCommand : ICommand {
        public ProgramData Execute(ProgramData data) {
            ConsoleKeyInfo key = Console.ReadKey();
            data.ByteCells[data.Pointer] = (byte)key.KeyChar;
            return new ProgramData {
                Pointer = data.Pointer,
                ByteCells = data.ByteCells
            };
        }

        public override string ToString() => ",";
    }

    public class LoopCommand : ICommand {
        private List<ICommand> _commands = new List<ICommand>();
        private ProgramData _currentData;
        private int? _currentLoopIndex;

        public ProgramData Execute(ProgramData data) {
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
