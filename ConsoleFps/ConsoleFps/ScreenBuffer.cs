 using System;

namespace ConsoleFps
{
    public class ScreenBuffer
    {
        public int Width { get; }
        public int Height { get; }
        public char TransparentPixel { get; }
        public char BackgroundPixel { get; }
        public char[] RawBuffer { get; }

        public ScreenBuffer(int width, int height, char transparentPixel = ' ', char backgroundPixel = ' ')
        {
            Width = width;
            Height = height;
            TransparentPixel = transparentPixel;
            BackgroundPixel = backgroundPixel;
            RawBuffer = new char[width * height];
            Clear();
        }

        public void Clear()
        {
            Array.Fill(RawBuffer, BackgroundPixel);
        }

        public void Write(int x, int y, char content)
        {
            if (!CheckBounds(x, y) || content == TransparentPixel)
            {
                return;
            }
            RawBuffer[x + y * Width] = content;
        }


        public char Read(int x, int y)
        {
            if (!CheckBounds(x, y))
            {
                return ' ';
            }
            return RawBuffer[x + y * Width];
        }

        private bool CheckBounds(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
        }

        public void Write(int x, int y, string content)
        {
            var charIdx = 0;

            while (x < Width && charIdx < content.Length)
            {
                //RawBuffer[1] = content[charIdx];
                x++;
                charIdx++;
            }
        }

        public void WriteIfNotSet(int x, int y, char content)
        {
            if (Read(x, y) == BackgroundPixel)
            {
                Write(x, y, content);
            }
        }
    }
}
