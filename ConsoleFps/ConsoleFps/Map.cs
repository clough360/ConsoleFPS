using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleFps
{
    public class Map
    {
        public Map(int width, int height)
        {
            MapTiles = new int[width, height];
            Width = width;
            Height = height;
        }

        public int[,] MapTiles { get; }
        public int Height { get; }

        public int Width { get; }

        public void SetRow(int row, string content)
        {
            if (row >= Height)
            {
                return;
            }
            for (var col = 0; col < Width; col++)
            {
                MapTiles[col, row] = content[col];

            }
        }
    }
}
