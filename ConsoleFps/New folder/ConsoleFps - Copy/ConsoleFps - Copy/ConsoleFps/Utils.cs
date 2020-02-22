using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleFps
{
    public static class Utils
    {
        public static double CircleRadians = 2d * Math.PI;
        public static double RationaliseAngle(double angle)
        {
            while (angle < 0)
            {
                angle += CircleRadians;
            }
            while (angle > CircleRadians)
            {
                angle -= CircleRadians;
            }
            return angle;
        }
    }
}
