using System;
using System.Runtime.InteropServices;

namespace ConsoleGameEngine
{
    public static class WindowsApi
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;

        public const uint CONSOLE_TEXTMODE_BUFFER = 0x00000001;

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;

        public const int STD_OUTPUT_HANDLE = -11;
        public const int TMPF_TRUETYPE = 4;
        public const int LF_FACESIZE = 32;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

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
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;

            public SmallRect(short left, short top, short right, short bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

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

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            char[] lpCharacter,
            [MarshalAs(UnmanagedType.U4)] uint nLength,
            Coord dwWriteCoord,
            out IntPtr lpNumberOfCharsWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal uint nFont;
            internal Coord dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }

        public static void CheckWinApiSuccess()
        {
            Int32 err = Marshal.GetLastWin32Error();
            if (err != 0)
            {
                Console.WriteLine("Error: {0}", err);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleWindowInfo(
            IntPtr hConsoleOutput,
            bool bAbsolute,
            ref SmallRect lpConsoleWindow
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferSize(
            IntPtr hConsoleOutput,
            Coord dwSize
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Coord GetLargestConsoleWindowSize(
            IntPtr hConsoleOutput
        );
    }
}
