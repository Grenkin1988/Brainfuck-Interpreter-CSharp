using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BrainfuckInterpreterCSharpStatic {
    public static class BrainfuckInterpreter {
        private const char MovePointerToRightCommand = '>';
        private const char MovePointerToLeftCommand = '<';
        private const char IncreaseCurrentCellCommand = '+';
        private const char DecreaseCurrentCellCommand = '-';
        private const char PrintCurrentCellCommand = '.';
        private const char ReadToCurrentCellCommand = ',';
        private const char LoopStartCommand = '[';
        private const char LoopEndCommand = ']';

        public static void Run(string program) {
            char[] cleanProgram = program.CleanInputProgram().ToCharArray();
            uint nextCommandIndex = 0;
            var byteCells = new byte[5];
            uint pointer = 0;

            ProcessNextCommand(cleanProgram, nextCommandIndex, byteCells, pointer);
        }

        private static string CleanInputProgram(this string program) {
            return Regex.Replace(program, @"[^><+-.,[\]]", string.Empty);
        }

        private static void ProcessNextCommand(
            char[] program,
            uint nextCommandIndex,
            byte[] byteCells,
            uint pointer) {
            switch (program[nextCommandIndex]) {
                case MovePointerToRightCommand: {
                    (byteCells, pointer)
                        = ExecuteMovePointerToRightCommand(byteCells, pointer);
                    nextCommandIndex++;
                    break;
                }
                case MovePointerToLeftCommand: {
                    pointer = ExecuteMovePointerToLeftCommand(pointer);
                    nextCommandIndex++;
                    break;
                }
                case IncreaseCurrentCellCommand: {
                    byteCells = ExecuteIncreaseCurrentCellCommand(byteCells, pointer);
                    nextCommandIndex++;
                    break;
                }
                case DecreaseCurrentCellCommand: {
                    byteCells = ExecuteDecreaseCurrentCellCommand(byteCells, pointer);
                    nextCommandIndex++;
                    break;
                }
                case PrintCurrentCellCommand: {
                    ExecutePrintCurrentCellCommand(byteCells, pointer);
                    nextCommandIndex++;
                    break;
                }
                case ReadToCurrentCellCommand: {
                    byteCells = ExecuteReadToCurrentCellCommand(byteCells, pointer);
                    nextCommandIndex++;
                    break;
                }
                case LoopStartCommand: {
                    if (byteCells[pointer] == 0) {
                        nextCommandIndex = FindCorespondingLoopEnd(program, nextCommandIndex);
                    } else {
                        nextCommandIndex++;
                    }

                    break;
                }
                case LoopEndCommand: {
                    if (byteCells[pointer] != 0) {
                        nextCommandIndex = FindCorespondingLoopStart(program, nextCommandIndex);
                    } else {
                        nextCommandIndex++;
                    }

                    break;
                }
            }

            if (nextCommandIndex < program.Length) {
                ProcessNextCommand(program, nextCommandIndex, byteCells, pointer);
            }
        }

        private static (byte[] byteCells, uint pointer) ExecuteMovePointerToRightCommand(byte[] byteCells, uint pointer) {
            pointer++;
            if (pointer >= byteCells.Length) {
                byteCells = AddNewCell(byteCells);
            }

            return (byteCells, pointer);
        }

        private static byte[] AddNewCell(byte[] byteCells) {
            var newByteCells = new byte[byteCells.Length + 1];
            for (int i = 0; i < byteCells.Length; i++) {
                newByteCells[i] = byteCells[i];
            }

            return newByteCells;
        }

        private static uint ExecuteMovePointerToLeftCommand(uint pointer) {
            pointer = pointer > 0 ? pointer - 1 : pointer;
            return pointer;
        }

        private static byte[] ExecuteIncreaseCurrentCellCommand(byte[] byteCells, uint pointer) {
            byteCells[pointer]++;
            return byteCells;
        }

        private static byte[] ExecuteDecreaseCurrentCellCommand(byte[] byteCells, uint pointer) {
            byteCells[pointer]--;
            return byteCells;
        }

        private static void ExecutePrintCurrentCellCommand(byte[] byteCells, uint pointer) {
            Console.Write((char)byteCells[pointer]);
        }

        private static byte[] ExecuteReadToCurrentCellCommand(byte[] byteCells, uint pointer) {
            ConsoleKeyInfo key = Console.ReadKey();
            byteCells[pointer] = (byte)key.KeyChar;
            return byteCells;
        }

        private static uint FindCorespondingLoopEnd(char[] program, uint currentCommandIndex) {
            uint innerLoopsCount = 0;
            for (uint i = currentCommandIndex + 1; i < program.Length; i++) {
                switch (program[i]) {
                    case LoopStartCommand: {
                        innerLoopsCount++;
                        break;
                    }
                    case LoopEndCommand: {
                        if (innerLoopsCount == 0) {
                            return i;
                        }

                        innerLoopsCount--;
                        break;
                    }
                }
            }

            throw new InvalidDataException(@"Programm do not have coresponding closing ""]""");
        }

        private static uint FindCorespondingLoopStart(char[] program, uint currentCommandIndex) {
            uint innerLoopsCount = 0;
            for (int i = (int)currentCommandIndex - 1; i >= 0; i--) {
                switch (program[i]) {
                    case LoopEndCommand: {
                        innerLoopsCount++;
                        break;
                    }
                    case LoopStartCommand: {
                        if (innerLoopsCount == 0) {
                            return (uint)i;
                        }

                        innerLoopsCount--;
                        break;
                    }
                }
            }

            throw new InvalidDataException(@"Programm do not have coresponding closing ""]""");
        }
    }
}
