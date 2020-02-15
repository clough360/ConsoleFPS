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

        private const double _circle = 2d * Math.PI;
        public void Move(double distance)
        {
            X +=  distance * Math.Cos(Angle);
            Y +=  distance * Math.Sin(Angle);
        }

        public void Rotate(double radians)
        {
            Angle += radians;
            if (Angle < 0)
            {
                Angle = Angle += _circle;
            }
            if (Angle > _circle)
            {
                Angle -= _circle;
            }
        }
    }
}
