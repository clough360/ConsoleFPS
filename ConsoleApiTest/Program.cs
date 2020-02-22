using System;
using System.Runtime.InteropServices;
using ConsoleGameEngine;
using Microsoft.Win32.SafeHandles;

namespace ConsoleApiTest
{
    class Program
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateConsoleScreenBuffer(
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr securityAttributes,
            uint flags,
            IntPtr lpScreenBufferData);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetConsoleActiveScreenBuffer(
            IntPtr hConsoleOutput);

        public static uint GENERIC_READ = 0x80000000;
        public static uint GENERIC_WRITE = 0x4000000;

        public const uint CONSOLE_TEXTMODE_BUFFER = 0x00000001;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            char[] lpCharacter,
            [MarshalAs(UnmanagedType.U4)] uint nLength,
            Coord dwWriteCoord,
            out IntPtr lpNumberOfCharsWritten);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        static void Main(string[] args)
        {
            //var h = CreateConsoleScreenBuffer(0x40000000, 0x00000000, IntPtr.Zero, 1, IntPtr.Zero);
            var h = ConsoleApi.GetNewConsoleHandle();
            ConsoleApi.SetConsoleFont(h, "Lucidia Console", 5);
            ConsoleApi.SetConsoleSize(h, 100, 10);
            CheckWinApiSuccess();
            SetConsoleActiveScreenBuffer(h);
            CheckWinApiSuccess();

            //displayChar = '\u2588';
            //displayChar = '\u2593';
            //displayChar = '\u2592';
            //displayChar = '\u2591';

            var screen = new char[120*30];
            Array.Fill(screen, '\u2588');
            WriteConsoleOutputCharacter(h, screen, 120 * 30, new Coord(0,0), out var bytesWritten);
            CheckWinApiSuccess();

            Console.WriteLine(Console.LargestWindowHeight);
            Console.WriteLine(Console.LargestWindowWidth);
            Console.ReadKey();
        }

        private static void CheckWinApiSuccess()
        {
            Int32 err = Marshal.GetLastWin32Error();
            if (err != 0)
            {
                Console.WriteLine("Error: {0}", err);
            }
        }

    }
}
