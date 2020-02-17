namespace ConsoleFps
{
    public class Sprite
    {
        public Sprite(int width, int height)
        {
            Pixels = new int[width, height];
            Width = width;
            Height = height;
        }

        public int[,] Pixels { get; }
        public int Height { get; }

        public int Width { get; }

        public void SetRow(int row, string content)
        {
            if (row >= Height)
            {
                return;
            }
            for (var col = 0; col < Width && col < content.Length; col++)
            {
                Pixels[col, row] = content[col];
            }
        }
    }
}
