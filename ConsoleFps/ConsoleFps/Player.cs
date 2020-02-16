using System;

namespace ConsoleFps
{
    public class Player
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        /// <summary>
        /// angle in radians (2pi rads per circle)
        /// </summary>
        public double Angle { get; set; } = 0;

        public bool Move(double distance, Map map)
        {
            var newX = X + distance * Math.Cos(Angle);
            var newY = Y + distance * Math.Sin(Angle);
            if (map.MapTiles[(int) newX, (int) newY] == ' ')
            {
                X = newX;
                Y = newY;
                return true;
            }
            return false;
        }

        public void Rotate(double radians)
        {
            Angle = Utils.RationaliseAngle(Angle + radians);
        }
    }
}
