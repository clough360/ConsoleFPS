using System;
using NUnit.Framework;

namespace ConsoleFps.Tests
{
    [TestFixture]
    public class TestConsoleFps
    {
        [TestCase(0.2, 0, 0.8, TestName = "top face 1")]
        [TestCase(0.5, 0, 0.5, TestName = "top face 2")]
        [TestCase(0, 0.2, 0.2, TestName = "left face")]
        [TestCase(0.9, 0.2, 0.8, TestName = "right face")]
        [TestCase(0.2, 0.99, 0.2, TestName = "bottom face")]
        public void CalculateBoxHitProportion(double x, double y, double result)
        {
            Assert.That(Program.CalculateBoxHitProportion(x, y), Is.EqualTo(result).Within(0.02));
        }

    }
}
