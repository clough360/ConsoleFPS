using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ConsoleGameEngine
{
    public static class ConsoleApi
    {
        /// <summary>
        /// get the handle of a new console screen
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetNewConsoleHandle()
        {
            //var h = WindowsApi.CreateConsoleScreenBuffer(WindowsApi.GENERIC_WRITE, 0x00000000, IntPtr.Zero, WindowsApi.CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);
            var h = WindowsApi.CreateConsoleScreenBuffer(WindowsApi.GENERIC_WRITE, 0x00000000, IntPtr.Zero, 1, IntPtr.Zero);
            WindowsApi.CheckWinApiSuccess();
            return h;
        }

        /// <summary>
        /// sets the active console to the one represented by handle
        /// </summary>
        /// <param name="h"></param>
        public static void SetActiveConsole(IntPtr h)
        {
            WindowsApi.SetConsoleActiveScreenBuffer(h);
        }

        public static void WriteWholeScreen(IntPtr h, char[] screenRawBuffer)
        {
            WindowsApi.WriteConsoleOutputCharacter(h, screenRawBuffer, (uint)screenRawBuffer.Length, new WindowsApi.Coord(0,0), out var bytesWritten);
        }

        public static void SetConsoleFont(IntPtr h, string fontName = "Lucida Console", short fontSize = 10)
        {
            unsafe
            {
                if (h != WindowsApi.INVALID_HANDLE_VALUE)
                {
                    var info = new WindowsApi.CONSOLE_FONT_INFO_EX();
                    info.cbSize = (uint)Marshal.SizeOf(info);

                    var newInfo = new WindowsApi.CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = WindowsApi.TMPF_TRUETYPE;
                    var ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                    // Get some settings from current font.
                    //newInfo.dwFontSize = new WindowsApi.Coord(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.dwFontSize = new WindowsApi.Coord(fontSize, fontSize);
                    newInfo.FontWeight = info.FontWeight;
                    WindowsApi.SetCurrentConsoleFontEx(h, false, ref newInfo);
                }
            }
        }

        public static bool SetConsoleSize(IntPtr h, short width, short height)
        {
            var rect = new WindowsApi.SmallRect(0, 0, (short)(width-1), (short)(height-1));
            WindowsApi.SetConsoleScreenBufferSize(h, new WindowsApi.Coord(width, height));
            return WindowsApi.SetConsoleWindowInfo(h, true, ref rect);
        }
    }
}
