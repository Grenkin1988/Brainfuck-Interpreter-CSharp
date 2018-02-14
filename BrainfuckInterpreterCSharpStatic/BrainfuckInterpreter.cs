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
            (char[] program, uint nextCommandIndex, byte[] byteCells, uint pointer) result 
                = ProcessNextCommand(cleanProgram, nextCommandIndex, byteCells, pointer);
        }

        private static string CleanInputProgram(this string program) {
            return Regex.Replace(program, @"[^><+-.,[\]]", string.Empty);
        }

        private static (char[] program, uint nextCommandIndex, byte[] byteCells, uint pointer) ProcessNextCommand(
            char[] program,
            uint nextCommandIndex,
            byte[] byteCells,
            uint pointer) {
            (char[] program, uint nextCommandIndex, byte[] byteCells, uint pointer) result
                = (program, nextCommandIndex, byteCells, pointer);

            switch (program[nextCommandIndex]) {
                case MovePointerToRightCommand: {
                    (result.byteCells, result.pointer)
                        = ExecuteMovePointerToRightCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case MovePointerToLeftCommand: {
                    (result.byteCells, result.pointer)
                        = ExecuteMovePointerToLeftCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case IncreaseCurrentCellCommand: {
                    (result.byteCells, result.pointer)
                        = ExecuteIncreaseCurrentCellCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case DecreaseCurrentCellCommand: {
                    (result.byteCells, result.pointer)
                        = ExecuteDecreaseCurrentCellCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case PrintCurrentCellCommand: {
                    (result.byteCells, result.pointer)
                        = ExecutePrintCurrentCellCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case ReadToCurrentCellCommand: {
                    (result.byteCells, result.pointer)
                        = ExecuteReadToCurrentCellCommand(result.byteCells, result.pointer);
                    result.nextCommandIndex++;
                    break;
                }
                case LoopStartCommand: {
                    if (result.byteCells[result.pointer] == 0) {
                        result.nextCommandIndex = FindCorespondingLoopEnd(result.program, result.nextCommandIndex);
                    } else {
                        result.nextCommandIndex++;
                    }
                    break;
                }
                case LoopEndCommand: {
                    if (result.byteCells[result.pointer] != 0) {
                        result.nextCommandIndex = FindCorespondingLoopStart(result.program, result.nextCommandIndex);
                    } else {
                        result.nextCommandIndex++;
                    }
                    break;
                }
            }

            return result.nextCommandIndex < program.Length ? 
                ProcessNextCommand(result.program, result.nextCommandIndex, result.byteCells, result.pointer) 
                : result;
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

        private static (byte[] byteCells, uint pointer) ExecuteMovePointerToLeftCommand(byte[] byteCells, uint pointer) {
            pointer = pointer > 0 ? pointer - 1 : pointer;
            return (byteCells, pointer);
        }

        private static (byte[] byteCells, uint pointer) ExecuteIncreaseCurrentCellCommand(byte[] byteCells, uint pointer) {
            byteCells[pointer]++;
            return (byteCells, pointer);
        }

        private static (byte[] byteCells, uint pointer) ExecuteDecreaseCurrentCellCommand(byte[] byteCells, uint pointer) {
            byteCells[pointer]--;
            return (byteCells, pointer);
        }

        private static (byte[] byteCells, uint pointer) ExecutePrintCurrentCellCommand(byte[] byteCells, uint pointer) {
            Console.Write((char)byteCells[pointer]);
            return (byteCells, pointer);
        }

        private static (byte[] byteCells, uint pointer) ExecuteReadToCurrentCellCommand(byte[] byteCells, uint pointer) {
            ConsoleKeyInfo key = Console.ReadKey();
            byteCells[pointer] = (byte)key.KeyChar;
            return (byteCells, pointer);
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
