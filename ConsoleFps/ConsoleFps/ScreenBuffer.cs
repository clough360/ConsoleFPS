using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConsoleFps
{
    public class ScreenBuffer
    {
        private int[,] _screen;
        public int Width { get; }
        public int Height { get; }

        public ScreenBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            _screen = new int[width,height];
            Clear();
        }

        private void Clear()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    _screen[x, y] = ' ';
                }
            }
        }

        public void Write(int x, int y, int content)
        {
            if (!CheckBounds(x, y))
            {
                return;
            }
            _screen[x, y] = content;
        }

        public int Read(int x, int y)
        {
            if (!CheckBounds(x, y))
            {
                return ' ';
            }
            return _screen[x, y];
        }

        private bool CheckBounds(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Width);
        }

        public void Write(int x, int y, string content)
        {
            var charIdx = 0;
            while (x < Width && charIdx < content.Length)
            {
                _screen[x, y] = content[charIdx];
                x++;
                charIdx++;
            }
        }
    }
}
