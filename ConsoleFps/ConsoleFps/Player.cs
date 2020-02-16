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

        public void Move(double distance)
        {
            X +=  distance * Math.Cos(Angle);
            Y +=  distance * Math.Sin(Angle);
        }

        public void Rotate(double radians)
        {
            Angle = Utils.RationaliseAngle(Angle + radians);
        }
    }
}
