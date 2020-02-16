using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ConsoleFps
{
    class Program
    {

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        private const double _rotationAngle = Math.PI / 16;
        private const double _stepSize = 0.5d;
        private const double _fieldOfView = Math.PI / 2;
        private const double _fieldDepth = 16;
        private const char _emptyMapSpace = ' ';

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var width = 100;
            var height = 30;
            var frameTimer = new Stopwatch();
            var lastFrameTime = 0l;

            var map = new Map(16,16);
            var row = 0;
            map.SetRow(row++,"1111111111111111");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"11111111       1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"111111   1111111");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1111111111111111");

            var screen = new ScreenBuffer(120, 40);
            var player = new Player()
            {
                X = 5, Y=5,
            };

            var executing = true;
        
            while (executing)
            {
                frameTimer.Restart();
                screen.Clear();
                RenderScene(player, map, _fieldOfView, _fieldDepth, width, height, screen);
                WriteDebugInfo(screen, width + 1, lastFrameTime, map, player, _fieldOfView);
                DrawScreenBuffer(screen);
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
                if (IsKeyPushedDown('X'))
                {
                    executing = false;
                }
            }

        }

        private static void RenderScene(Player player, Map map, double fov, double fieldDepth, int viewPortWidth, int viewPortHeight, ScreenBuffer screen)
        {
            var increment = fov / viewPortWidth;
            var rayAngleRelative = 0 - (fov / 2d);
            for (var x = 0; x < viewPortWidth; x++)
            {
                for (var depth = 1d; depth < fieldDepth; depth+= 0.2)
                {
                    var rayAngle = Utils.RationaliseAngle(player.Angle + rayAngleRelative);
                    var tileX = player.X + depth * Math.Cos(rayAngle);
                    var tileY = player.Y + depth * Math.Sin(rayAngle);
                    if (!map.CheckBounds(tileX, tileY))
                    {
                        break;
                    }
                    if (map.MapTiles[(int)tileX, (int)tileY] != _emptyMapSpace)
                    {
                        // wall
                        DrawWall(x, depth, viewPortHeight, screen);
                        break;
                    }

                }
                rayAngleRelative += increment;
            }
            
        }

        private static void DrawWall(int x, double depthWithinField, int viewPortHeight, ScreenBuffer screen)
        {
            var height = 2 * viewPortHeight / depthWithinField;
            var yStart = (int)((viewPortHeight - height)/2d);

            for (var y = 0; y < viewPortHeight; y++)
            {
                var displayChar = ' ';

                // ceiling
                if (y < yStart)
                {
                    if (y < 4)
                    {
                        displayChar = '\u2584';
                    }
                    else if (y < 8)
                    {
                        displayChar = '\u2583';
                    }
                    else if (y < 12)
                    {
                        displayChar = '\u2582';
                    }
                    else if (y < 16)
                    {
                        displayChar = '\u2581';
                    }
                }
                // wall
                else if (y >= yStart && y <= yStart + height)
                {
                    if (depthWithinField < 3)
                    {
                        displayChar = '\u2588';
                    }
                    else if (depthWithinField < 8)
                    {
                        displayChar = '\u2593';
                    }
                    else if (depthWithinField < 12)
                    {
                        displayChar = '\u2592';
                    }
                    else if (depthWithinField < 16)
                    {
                        displayChar = '\u2591';
                    }
                }
                // floor
                else
                {
                    var floorY = y - yStart - height;
                    if (floorY < 4)
                    {
                        displayChar = '\u2581';
                    }
                    else if (floorY < 8)
                    {
                        displayChar = '\u2582';
                    }
                    else if (floorY < 12)
                    {
                        displayChar = '\u2583';
                    }
                    else if (floorY < 16)
                    {
                        displayChar = '\u2584';
                    }
                }

                screen.Write(x, y, displayChar);

            }
            for (var dy = 0; dy < height; dy++)
            {
                var screenY = yStart + dy;

            
            }

        }

        private static void DrawScreenBuffer(ScreenBuffer screen)
        {
            for (var y = 0; y < Console.WindowHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                for (var x = 0; x < Console.WindowWidth; x++)
                {
                    Console.Write((char) screen.Read(x, y));
                }
            }
        }

        public static bool IsKeyPushedDown(char key)
        {
            return 0 != (GetAsyncKeyState((ushort)key) & 0x8000);
        }


        private static void WriteDebugInfo(ScreenBuffer screen , int debugStartX, long lastFrameTime, Map map, Player player, double fov)
        {
            var row = 0;
            screen.Write(debugStartX + 1, row++, $"FPS: {Math.Round(1000f / lastFrameTime, 1)}");
            screen.Write(debugStartX + 1, row++, $"FT: {lastFrameTime}ms");
            screen.Write(debugStartX + 1, row++, $"P: {player.X:F1}, {player.Y:F1}, {player.Angle:F1}");

            var mapStartY = Console.WindowHeight - map.Height;

            DrawMapOnScreen(screen, debugStartX, mapStartY, map, player);

            var mapOverlay = new Map(map.Width, map.Height);
            // draw field of view on map
            var fovStartAngle = Utils.RationaliseAngle(player.Angle - fov / 2d);
            var fovEndAngle = Utils.RationaliseAngle(fovStartAngle + fov);

            DrawLineOnMap(mapOverlay, player.X, player.Y, fovStartAngle);
            DrawLineOnMap(mapOverlay, player.X, player.Y, fovEndAngle);
            DrawMapOnScreen(screen, debugStartX, mapStartY, mapOverlay, player);
        }

        private static void DrawMapOnScreen(ScreenBuffer screen, int screenStartX, int screenStartY, Map map, Player player, char transparentChar = '\0')
        {
            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    if (x == (int) player.X && y == (int) player.Y)
                    {
                        screen.Write(screenStartX + x, y + screenStartY, 'X');
                        continue;
                    }
                    var tileContents = ((char) map.MapTiles[x, y]);
                    if (tileContents != transparentChar)
                    {
                        screen.Write(screenStartX + x, y + screenStartY, tileContents);
                    }
                }
            }
        }

        private static void DrawLineOnMap(Map map, double startX, double startY, double angle)
        {

            var inMap = true;
            var depth = 1;
            while (inMap)
            {
                var tileX = (int) (startX + depth * Math.Cos(angle));
                var tileY = (int) (startY + depth * Math.Sin(angle));
                if (!map.CheckBounds(tileX, tileY))
                {
                    inMap = false;
                }
                else
                {
                    map.MapTiles[tileX, tileY] = '.';
                }
                depth++;
            }
        }
    }

    
}
