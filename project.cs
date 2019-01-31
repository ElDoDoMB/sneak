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
        public const char Plus = '+';
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
                    if ((X == 0 || X == XMax - 1) && (Y == 0 || Y == YMax - 1))
                    {
                        list.Add(S.Plus);
                    }
                    else if (Y < 1 || Y > YMax - 2) list.Add(S.XWall);
                    else if (X < 1 || X > XMax - 2) list.Add(S.YWall);
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
            return true;
            else return false;
        }

        private void DrawSneak(char Sneak, char Tail, List<Sneak> SneaKN) 
        {
            for (int i = 1; i < SneaKN.Count; i++)
            {
                MaP[SneaKN[i].YPr][SneaKN[i].XPr] = S.Empty;
                SneaKN[i].Draw(Tail);
                SneaKN[i].Y = SneaKN[i - 1].YPr;
                SneaKN[i].X = SneaKN[i - 1].XPr;
            }
            SneaKN[0].Draw(Sneak);
            WriteMap();
        }

        private void WriteMap() 
        {
            var strBuilder = new StringBuilder(YMax * XMax);
            for (int Y = 0; Y < YMax; Y++)
            {
                for (int X = 0; X < XMax; X++)
                {
                    strBuilder.Append(MaP[Y][X]);
                }
                strBuilder.AppendLine();
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();
            Console.Write(strBuilder.ToString());
        }

        public void Draw()
        {
            if (isCrash())
            {
                DrawSneak(S.Empty, S.Empty, SneaK);
                Program.StopGame();
            }
            else
            {
                if (MaP[SneaK[0].Y][SneaK[0].X] == S.Food)
                {
                    MaP[SneaK[0].Y][SneaK[0].X] = S.Empty;
                    FooD.Draw();
                    SneaK.Add(new Sneak(SneaK[SneaK.Count - 1].XPr, SneaK[SneaK.Count - 1].YPr));
                }
                DrawSneak(S.Sneak, S.Tail, SneaK);
            }

            
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
                if (X != XPr || Y != YPr)
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
            do
            {
                X = RanD.Next(1, Map.XMax - 2);
                Y = RanD.Next(1, Map.YMax - 2);
            }
            while (Map.MaP[Y][X] != S.Empty);

            Map.MaP[Y][X] = S.Food;
        }

    }

    class Program
    {
        public static bool gameStatus = true;
        public static Map GameMap;
        public static ConsoleKeyInfo KeyInfoN;
        public static Direction D;


        static void StartGame()
        {
            GameMap = new Map();
            while (gameStatus)
            {
                D = KeyToDirection(KeyInfoN.Key);
                if (KeyInfoN.Key == ConsoleKey.X) StopGame();

                do
                {
                    InfoGame(GameInfo.inGame);
                    Console.SetCursorPosition(0, 1);
                    GameMap.Draw();
                }
                while ((!Console.KeyAvailable || isAntiKey()) && gameStatus);

            }
        }

        static void Main()
        {
            if (gameStatus)
            {
                InfoGame(GameInfo.Start);
                Console.ReadKey(true);
                Console.Clear();
                StartGame();
            }
        }

        public static void StopGame()
        {
            InfoGame(GameInfo.Stop);
            KeyInfoN = Console.ReadKey(true);
            if (KeyInfoN.Key == ConsoleKey.X)
                gameStatus = false;
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
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Press any key to start...");
                    break;
                case GameInfo.inGame:
                    Console.SetCursorPosition(0, Map.YMax + 2);
                    Console.WriteLine("Score - " + Map.SneaK.Count);
                    /*for (int i = 0; i < Map.SneaK.Count; i++)
                    {
                        int N = i + 1;
                        Console.WriteLine(N + " - " + Map.SneaK[i].X + " , " + Map.SneaK[i].Y);
                        Console.WriteLine(N + "Prev - " + Map.SneaK[i].XPr + " , " + Map.SneaK[i].YPr);
                    }*/
                    break;
                case GameInfo.Stop:
                    Console.SetCursorPosition(0, Map.YMax + 2);
                    Console.WriteLine("GAME OVER with score " + Map.SneaK.Count);
                    Console.WriteLine("Press any key to restart.");
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
