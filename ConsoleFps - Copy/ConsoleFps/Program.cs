using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace ConsoleFps
{
    

    class Program
    {


        private const double _rotationAngle = Math.PI / 16;
        private const double _stepSize = 0.5d;
        private const double _fieldOfView = Math.PI / 2;
        private const double _fieldDepth = 16;
        private const char _emptyMapSpace = ' ';
        private const int _ticksPerSecond = 10000*1000; //10k ticks per ms

        static void Main(string[] args)
        {
            //SafeFileHandle h = Windows.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            var h = Windows.CreateConsoleScreenBuffer(Windows.GENERIC_READ | Windows.GENERIC_WRITE, 0x00000003, IntPtr.Zero,  1, IntPtr.Zero);
            //var h = Windows.CreateConsoleScreenBuffer(Windows.GENERIC_READ | Windows.GENERIC_WRITE, Windows.FILE_SHARE_WRITE | Windows.FILE_SHARE_READ, IntPtr.Zero,  Windows.CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);
            //CheckWinApiSuccess();
            //Windows.SetConsoleActiveScreenBuffer(h);
            //CheckWinApiSuccess();

            // Verifying the PInvoke worked.


            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var width = 100;
            var height = 30;
            var frameTimer = new Stopwatch();
            var lastFrameTime = 0l;
            var isAlive = true;

            var map = new Map(16,16);
            var row = 0;
            map.SetRow(row++,"1111111111111111");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"11111111       1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1     111      1");
            map.SetRow(row++,"1       1      1");
            map.SetRow(row++,"1       1      1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"111111   1111111");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1              1");
            map.SetRow(row++,"1111111111111111");

            row = 0;
            var badGuySprite = new Sprite(10, 20);
            badGuySprite.SetRow(row++, "          ");
            badGuySprite.SetRow(row++, "          ");
            badGuySprite.SetRow(row++, "          ");
            badGuySprite.SetRow(row++, "          ");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "   XXXX   ");
            badGuySprite.SetRow(row++, "   XXXX   ");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "   XXXX   ");
            badGuySprite.SetRow(row++, "   XXXX   ");
            badGuySprite.SetRow(row++, "   XXXX   ");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "XXXXXXXXXX");
            badGuySprite.SetRow(row++, "XXX    XXX");
            badGuySprite.SetRow(row++, "XXX    XXX");
            badGuySprite.SetRow(row++, "XXX    XXX");
            badGuySprite.SetRow(row++, "XXX    XXX");

            var screen = new ScreenBuffer(120, 30, ' ');
            var player = new Player()
            {
                X = 5, Y=5,
            };

            var badGuy = new Player()
            {
                X=9, Y=5, Sprite = badGuySprite,
            };
            var executing = true;

            while (executing && isAlive)
            {
                frameTimer.Restart();
                screen.Clear();
                RenderScene(player, badGuy, map, _fieldOfView, _fieldDepth, width, height, screen);
                WriteDebugInfo(screen, width + 1, lastFrameTime, map, player, badGuy, _fieldOfView);
                DrawScreenBuffer(screen);
                lastFrameTime = frameTimer.ElapsedTicks;

                var speedAdjustFactor = 1;//_ticksPerSecond / lastFrameTime / 50;

                if (IsKeyPushedDown('A'))
                {
                    player.Rotate(-_rotationAngle / speedAdjustFactor);
                }
                if (IsKeyPushedDown('W'))
                {
                    player.Move(_stepSize / speedAdjustFactor, map);
                }
                if (IsKeyPushedDown('S'))
                {
                    player.Rotate(_rotationAngle / speedAdjustFactor);
                }
                if (IsKeyPushedDown('Z'))
                {
                    player.Move(-_stepSize / speedAdjustFactor, map);
                }
                if (IsKeyPushedDown('X'))
                {
                    executing = false;
                }

                var buf = new CharInfo[120 * 30];
                var buf2 = new char[120 * 30];

                for (var x = 0; x < screen.Width; x++)
                {
                    for (var y = 0; y < screen.Height; y++)
                    {
                        buf[x + y * screen.Width].Char.UnicodeChar = (char)screen.Read(x,y);
                        buf[x + y * screen.Width].Attributes = 12;
                        buf2[x + y * screen.Width] = (char)screen.Read(x, y);
                    }
                }

                //MoveBadGuy(badGuy, map);
                isAlive = !((int)player.X == (int)badGuy.X && (int)player.Y == (int)badGuy.Y);

                //for (var i = 0; i < 120*30; i++)
                //{
                //    buf[i].Char.AsciiChar = scree;
                //    buf[i].Attributes = (short)(i % 15);
                //}

                //SmallRect rect = new SmallRect() {Left = 0, Top = 0, Right = 120, Bottom = 30};
                //bool b = Windows.WriteConsoleOutput(h, buf,
                //    new Coord() {X = 120, Y = 30},
                //    new Coord() {X = 0, Y = 0},
                //    ref rect);

                //Windows.WriteConsoleOutputCharacter(h, buf2, (uint)buf2.Length, new Coord(){X=0,Y=0}, out var bytesWritten);
            }

            if (!isAlive)
            {
                Console.WriteLine("Game over - the bad guy got you :(");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

        private static void MoveBadGuy(Player badGuy, Map map)
        {
            var changeAngle = new Random().Next(0, 10) > 8;

            if (changeAngle)
            {
                badGuy.Rotate((new Random().NextDouble() - 0.5) * _rotationAngle);
            }
            if (!badGuy.Move(0.3, map))
            {
                badGuy.Rotate(new Random().NextDouble() * Utils.CircleRadians);
            }

        }

        private static void CheckWinApiSuccess()
        {
            Int32 err = Marshal.GetLastWin32Error();
            if (err != 0)
            {
                Console.WriteLine("Error: {0}", err);
            }
        }

        private static void RenderScene(Player player, Player badGuy, Map map, double fov, double fieldDepth, int viewPortWidth, int viewPortHeight, ScreenBuffer screen)
        {
            var increment = fov / viewPortWidth;
            var rayAngleRelative = 0 - (fov / 2d);
            for (var x = 0; x < viewPortWidth; x++)
            {
                var doneBadGuy = false;
                for (var depth = 1d; depth < fieldDepth; depth+= 0.2)
                {
                    var rayAngle = Utils.RationaliseAngle(player.Angle + rayAngleRelative);
                    var rayX = player.X + depth * Math.Cos(rayAngle);
                    var rayY = player.Y + depth * Math.Sin(rayAngle);
                    if (!map.CheckBounds(rayX, rayY))
                    {
                        // do nothing
                        break;
                    }
                    if (map.MapTiles[(int)rayX, (int)rayY] != _emptyMapSpace)
                    {
                        // wall
                        DrawWall(x, depth, viewPortHeight, screen);
                        break;
                    }
                    if (!doneBadGuy && ((int)rayX == (int) badGuy.X && (int)rayY == (int) badGuy.Y))
                    {
                        // bad guy
                        DrawBadGuy(x, depth, viewPortHeight, screen, rayX, rayAngle, badGuy);
                        doneBadGuy = true;
                    }
                }
                rayAngleRelative += increment;
            }
        }

        private static void DrawBadGuy(int x, double depthWithinField, int viewPortHeight, ScreenBuffer screen, double badGuyRayHitX, double rayAngle, Player badGuy)
        {
            var height = 2 * viewPortHeight / depthWithinField;
            var yStart = (int)((viewPortHeight - height) / 2d);

            // which column (part) of bad guy is this?
            // the ray hit bad guy at rayAngle at rayHitX
            // badGyRayHitX should be between x and x+1, determine the proportion of the way through
            
            var xProportion = (badGuyRayHitX - (int)badGuy.X);
            var spriteCol = (int)(xProportion * badGuy.Sprite.Width);

            // always draw bad guy as if he was facing you, as he looks weird with perspective
            for ( var y = 0; y < height; y++)
            {
                var spriteY = (int)((badGuy.Sprite.Height / height) * y);
     
                if (spriteY >= badGuy.Sprite.Height)
                {
                    break;
                }
                screen.Write(x, y + yStart, badGuy.Sprite.Pixels[spriteCol, spriteY]);
            }
        }

        private static void DrawWall(int x, double depthWithinField, int viewPortHeight, ScreenBuffer screen)
        {
            var height = 2 * viewPortHeight / depthWithinField;
            var yStart = (int)((viewPortHeight - height)/2d);

            for (var y = 0; y < viewPortHeight; y++)
            {
                if (screen.Read(x, y) != ' ')
                {
                    continue;
                }
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

                screen.WriteIfNotSet(x, y, displayChar);

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
            return 0 != (Windows.GetAsyncKeyState((ushort)key) & 0x8000);
        }

        private static void WriteDebugInfo(ScreenBuffer screen , int debugStartX, long lastFrameTimeTicks, Map map, Player player, Player badGuy, double fov)
        {
            var row = 0;
            screen.Write(debugStartX + 1, row++, $"FPS: {Math.Round(1d/(lastFrameTimeTicks/(double)_ticksPerSecond), 1)}");
            screen.Write(debugStartX + 1, row++, $"FT: {lastFrameTimeTicks}t");
            screen.Write(debugStartX + 1, row++, $"P: {player.X:F1}, {player.Y:F1}, {player.Angle:F1}");

            var mapStartY = Console.WindowHeight - map.Height;

            DrawMapOnScreen(screen, debugStartX, mapStartY, map, player, badGuy);

            var mapOverlay = new Map(map.Width, map.Height);
            // draw field of view on map
            var fovStartAngle = Utils.RationaliseAngle(player.Angle - fov / 2d);
            var fovEndAngle = Utils.RationaliseAngle(fovStartAngle + fov);

            DrawLineOnMap(mapOverlay, player.X, player.Y, fovStartAngle);
            DrawLineOnMap(mapOverlay, player.X, player.Y, fovEndAngle);
            DrawMapOnScreen(screen, debugStartX, mapStartY, mapOverlay, player, badGuy);
        }

        private static void DrawMapOnScreen(ScreenBuffer screen, int screenStartX, int screenStartY, Map map, Player player, Player badGuy, char transparentChar = '\0')
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
                    if (x == (int)badGuy.X && y == (int)badGuy.Y)
                    {
                        screen.Write(screenStartX + x, y + screenStartY, '&');
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
