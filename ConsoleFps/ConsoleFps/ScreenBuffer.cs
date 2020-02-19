 using System;

namespace ConsoleFps
{
    public ref struct ScreenBuffer
    {
        public int Width;
        public int Height;
        public char TransparentPixel;
        public char BackgroundPixel;
        public readonly Span<char> RawBuffer;

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
            RawBuffer.Fill(BackgroundPixel);
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
            // these are ordered in the order they are most likely to occur
            return (y < Height && y >= 0 && x >= 0 && x < Width);
        }


        public void Write(int x, int y, char content)
        {
            if (content == TransparentPixel)
            {
                return;
            }
            RawBuffer[x + y * Width] = content;
        }

        public void Write(int x, int y, string content)
        {
            var slice = RawBuffer.Slice(x + y * Width, content.Length);
            var idx = 0;
            foreach (var c in content) 
            {
                slice[idx] = c;
            }
        }

        public void WriteIfNotSet(int x, int y, char content)
        {
            if (RawBuffer[x + Width * y] == BackgroundPixel)
            {
                RawBuffer[x + Width * y] = content;
            }
        }
    }
}
