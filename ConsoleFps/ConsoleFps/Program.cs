using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ConsoleFps
{
    class Program
    {

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        private const double _rotationAngle = Math.PI / 4;
        private const double _stepSize = 0.2d;

        static void Main(string[] args)
        {
            
            var width = 100;
            var height = 30;
            var frameTimer = new Stopwatch();
            var lastFrameTime = 0l;

            var map = new Map(16,16);
            var row = 0;
            map.SetRow(row++,"1111111111111111");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1....1111111...1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1..............1");
            map.SetRow(row++,"1111111111111111");

            var screen = new ScreenBuffer(120, 40);
            var player = new Player()
            {
                X = 5, Y=5,
            };

            char currentKey = '\0';

        
            while (true)
            {
                WriteDebugInfo(screen, width + 1, lastFrameTime, map, player, currentKey);

                frameTimer.Restart();
                for (var y = 0; y < Console.WindowHeight; y++)
                {
                    Console.SetCursorPosition(0, y);
                    for (var x = 0; x < Console.WindowWidth; x++)
                    {
                        Console.Write((char)screen.Read(x,y));
                    }
                }
                lastFrameTime = frameTimer.ElapsedMilliseconds;


                if (IsKeyPushedDown('A'))
                {
                    player.Rotate(-_rotationAngle);
                }
                if (IsKeyPushedDown('W'))
                {
                    player.Move(_stepSize);
                }
                if (IsKeyPushedDown('S'))
                {
                    player.Rotate(_rotationAngle);
                }
                if (IsKeyPushedDown('Z'))
                {
                    player.Move(-_stepSize);
                }
            }

        }

        public static bool IsKeyPushedDown(char key)
        {
            return 0 != (GetAsyncKeyState((ushort)key) & 0x8000);
        }


        private static void WriteDebugInfo(ScreenBuffer screen , int debugStartX, long lastFrameTime, Map map, Player player, char currentKey)
        {
            var row = 0;
            screen.Write(debugStartX + 1, row++, $"FPS: {Math.Round(1000f / lastFrameTime, 1)}");
            screen.Write(debugStartX + 1, row++, $"FT: {lastFrameTime}ms");
            screen.Write(debugStartX + 1, row++, $"P: {player.X:F1}, {player.Y:F1}, {player.Angle:F1}");

            var mapStartY = Console.WindowHeight - map.Height;

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    if (x == player.X && y == player.Y)
                    {
                        screen.Write(debugStartX+x, y + mapStartY, 'X');
                    }
                    else
                    {
                        //screen.Write(debugStartX+x, y + mapStartY, x.ToString("X1"));
                        screen.Write(debugStartX + x, y + mapStartY, ((char)map.MapTiles[x, y]).ToString());
                    }
                }
            }
        }
    }

    
}
