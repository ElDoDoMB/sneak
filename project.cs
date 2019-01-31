using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleGameSneak
{
    enum Direction : int
    {
        Up = 1,
        Right = 2,
        Down = -1,
        Left = -2,
        Stop = 0
    }

    public static class S
    {
        public const char Empty = ' ';
        public const char Sneak = 'O';
        public const char Tail = 'o';
        public const char Food = '*';
        public const char XWall = '-';
        public const char YWall = '|';
    }



    class Map
    {
        public static int XMax = 50, YMax = 40;
        public static List<List<char>> MaP;


        public static List<Sneak> SneaK;
        private Food FooD = new Food();

        public Map()
        {
            MaP = new List<List<char>>(YMax);
            for (int Y = 0; Y < YMax; Y++)
            {
                var list = new List<char>(XMax);
                for (int X = 0; X < XMax; X++)
                {
                    if ((X < 2 && Y < 2) || (X > XMax - 3 && Y > YMax - 3))
                    {
                        list.Add(S.YWall);
                        list.Add(S.YWall);
                        X++;
                    }
                    else if (Y < 2 || Y > YMax - 3) list.Add(S.XWall);
                    else if (X < 2 || X > XMax - 3) list.Add(S.YWall);
                    else list.Add(S.Empty);
                }
                MaP.Add(list);
            }
            FooD.Draw();
            SneaK = new List<Sneak>();
            SneaK.Add(new Sneak());

        }

        private bool isCrash()
        {
            if (MaP[SneaK[0].Y][SneaK[0].X] == S.XWall ||
                MaP[SneaK[0].Y][SneaK[0].X] == S.YWall ||
                MaP[SneaK[0].Y][SneaK[0].X] == S.Tail) 
            {
                SneaK[0].Y = SneaK[0].YPr;
                SneaK[0].X = SneaK[0].XPr;
                SneaK[0].Draw(S.Sneak);
                return true;
            }
            else return false;
        }

        public void Draw()
        {
            if (isCrash())
                Program.StopGame();
            else
                if (MaP[SneaK[0].Y][SneaK[0].X] == S.Food)
                {
                    MaP[SneaK[0].Y][SneaK[0].X] = S.Empty;
                    FooD.Draw();
                    SneaK.Add(new Sneak(SneaK[SneaK.Count - 1].XPr, SneaK[SneaK.Count - 1].YPr));
                }

            for (int i = 1; i < SneaK.Count; i++)
            {
                MaP[SneaK[i].YPr][SneaK[i].XPr] = S.Empty;
                SneaK[i].Draw(S.Tail);
                SneaK[i].Y = SneaK[i - 1].YPr;
                SneaK[i].X = SneaK[i - 1].XPr;
            }
            SneaK[0].Draw(S.Sneak);
            var strBuilder = new StringBuilder(YMax * XMax);
            for (int Y = 0; Y < YMax; Y++)
            {
                for (int X = 0; X < XMax; X++)
                {
                    strBuilder.Append(MaP[Y][X]);
                }
                strBuilder.AppendLine();
            }
            Console.Write(strBuilder.ToString());
            
        }
    }

    class Sneak
    {
        public int X, Y, XPr, YPr;
        public static int MoveSpeed = 100;
        public bool Upp = false;
        private int Tick = 0;
        private Direction DPrev = Direction.Stop;

        public Sneak()
        {
            X = Map.MaP[0].Count / 2;
            Y = Map.MaP.Count / 2;
            Map.MaP[Y][X] = S.Sneak;
        }

        public Sneak(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            Map.MaP[Y][X] = S.Tail;
        }

        public void Draw(char Ch)
        {
            Tick++;
            Map.MaP[Y][X] = Ch;
            if (Tick == MoveSpeed ||
                Program.D != DPrev)
            {
                XPr = X;
                YPr = Y;
                if (Ch == S.Sneak)
                {
                    switch (Program.D)
                    {
                        case Direction.Up: Y--; break;
                        case Direction.Down: Y++; break;
                        case Direction.Left: X--; break;
                        case Direction.Right: X++; break;
                    }
                }
                if(X != XPr || Y != YPr)
                    Map.MaP[YPr][XPr] = S.Empty;
                Tick = 0;
                DPrev = Program.D;
            }
        }
    }


    class Food
    {
        private int X, Y;

        private Random RanD = new Random();

        public void Draw()
        {
            X = RanD.Next(2, Map.XMax - 3);
            Y = RanD.Next(2, Map.YMax - 3);
            Map.MaP[Y][X] = S.Food;
        }

    }

    class Program
    {
        public static bool gameStatus = true;
        public static Map GameMap;
        public static ConsoleKeyInfo KeyInfoN;
        public static Direction D ;


        static void StartGame()
        {
            GameMap = new Map();
            while (true)
            {
                D = KeyToDirection(KeyInfoN.Key);
                if (KeyInfoN.Key == ConsoleKey.X) StopGame();

                do
                {
                    InfoGame(GameInfo.inGame);
                    Console.SetCursorPosition(0, 1);
                    GameMap.Draw();
                }
                while ((!Console.KeyAvailable || isAntiKey()));

            }
        }

        static void Main()
        {

            InfoGame(GameInfo.Start);
            Console.ReadKey(true);
            Console.Clear();
            StartGame();
        }

        public static void StopGame()
        {
            InfoGame(GameInfo.Stop);
            KeyInfoN = Console.ReadKey(true);
            if (KeyInfoN.Key != ConsoleKey.X)
                Main();
        }

        enum GameInfo : int
        {
            Start,
            Stop,
            inGame
        }

        static void InfoGame(GameInfo GI)
        {
            switch (GI)
            {
                case GameInfo.Start:
                    Console.Clear();
                    Console.SetCursorPosition(1, 1);
                    Console.WriteLine("Press any key to start...");
                    break;
                case GameInfo.inGame:
                    Console.SetCursorPosition(1, Map.YMax + 2);
                    Console.WriteLine("Score - " + Map.SneaK.Count);
                    /*for (int i = 0; i < Map.SneaK.Count; i++)
                    {
                        int N = i + 1;
                        Console.WriteLine(N + " - " + Map.SneaK[i].X + " , " + Map.SneaK[i].Y);
                        Console.WriteLine(N + "Prev - " + Map.SneaK[i].XPr + " , " + Map.SneaK[i].YPr);
                    }*/
                    break;
                case GameInfo.Stop:
                    Console.SetCursorPosition(1, Map.MaP.Count + 2);
                    Console.WriteLine("GAME OVER with score " + Map.SneaK.Count);
                    Console.SetCursorPosition(1, Map.MaP.Count + 3);
                    Console.WriteLine("Press any key to restart.");
                    Console.SetCursorPosition(1, Map.MaP.Count + 4);
                    Console.WriteLine("Press X to stop game.");
                    break;
            }
        }

        static bool isAntiKey()
        {
            Direction DN;
            KeyInfoN = Console.ReadKey(true);
            DN = KeyToDirection(KeyInfoN.Key);

            if ((int)D == (-1) * (int)DN)
            {
                return true;
            }
            else return false;
        }

        static Direction KeyToDirection(ConsoleKey Key)
        {
            switch (Key)
            {
                case ConsoleKey.W: return Direction.Up;
                case ConsoleKey.S: return Direction.Down;
                case ConsoleKey.A: return Direction.Left;
                case ConsoleKey.D: return Direction.Right;
                default: return D;
            }
        }
    }
}
