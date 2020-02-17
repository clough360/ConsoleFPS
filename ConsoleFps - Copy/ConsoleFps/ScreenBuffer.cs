namespace ConsoleFps
{
    public class ScreenBuffer
    {
        private int[,] _screen;
        public int Width { get; }
        public int Height { get; }
        public int TransparentPixel { get; }
        public int BackgroundPixel { get; }

        public ScreenBuffer(int width, int height, int transparentPixel = ' ', int backgroundPixel = ' ')
        {
            Width = width;
            Height = height;
            TransparentPixel = transparentPixel;
            BackgroundPixel = backgroundPixel;
            _screen = new int[width,height];
            Clear();
        }

        public void Clear()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    _screen[x, y] = BackgroundPixel;
                }
            }
        }

        public void Write(int x, int y, int content)
        {
            if (!CheckBounds(x, y) || content == TransparentPixel)
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
            return (x >= 0 && x < Width && y >= 0 && y < Height);
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

        public void WriteIfNotSet(int x, int y, char content)
        {
            if (Read(x, y) == BackgroundPixel)
            {
                Write(x,y,content);
            }
        }
    }
}
