using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SeleniumTest1
{


    public class Board
    {
        private readonly int[,] board;
        private readonly int boardSize;
        private int score;
        private static readonly Random random = new();

        public static readonly Tuple<int, int> LEFT = new(0, -1);
        public static readonly Tuple<int, int> RIGHT = new(0, 1);
        public static readonly Tuple<int, int> UP = new(-1, 0);
        public static readonly Tuple<int, int> DOWN = new(1, 0);
        private static readonly List<Tuple<int, int>> directions = new() { LEFT, UP, RIGHT, DOWN };

        public Board(int[,] theBoard)
        {
            boardSize = theBoard.GetLength(0);
            board = (theBoard.Clone() as int[,])!;
            score = 0;
        }

        public override string ToString()
        {
            string outStr = "";
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    outStr += board[i, j].ToString() + "\t";
                }
                outStr += "\n";
            }
            return outStr;
        }

        public int GetTotalScore() => score;

        public int this[int key1, int key2]
        {
            [return: NotNull]
            get => board[key1, key2];

            [param: NotNull]
            set => board[key1, key2] = value;
        }

        // Return list of tuples containing indexes of open tiles
        public List<Tuple<int, int>> GetOpenTiles()
        {
            List<Tuple<int, int>> openTiles = new();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == 0)
                    {
                        openTiles.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return openTiles;
        }

        // Add a random tile to the board
        // Throws exception if board is full
        // 90% - 2
        // 10% - 4
        public void AddTile(Tuple<int, int>? pos = null, int tileToAdd = 0)
        {
            if (pos == null)
            {
                List<Tuple<int, int>> openTiles = GetOpenTiles();
                if (openTiles.Count == 0)
                {
                    throw new Exception("Unable to add tile, board is full");
                }
                pos = openTiles[random.Next(openTiles.Count)];
            }

            if (tileToAdd == 0)
            {
                if (random.NextDouble() < 0.9)
                {
                    tileToAdd = 2;
                }
                else
                {
                    tileToAdd = 4;
                }
            }

            board[pos.Item1, pos.Item2] = tileToAdd;
        }

        // Slide all tiles towards direction, combining tiles that slide into each other
        // dir: tuple containing x,y modifier to move towards
        // Returns sum of newly combined tiles
        public Tuple<int, bool> Move(Tuple<int, int> dir, bool addNextTile = true)
        {
            bool[,] hadCollision = new bool[boardSize, boardSize];
            bool hadMovement = false;
            int moveScore = 0;

            int xStart = 0;
            int xEnd = boardSize;
            if (dir.Item2 > 0)
            {
                xStart = boardSize - 1;
                xEnd = -1;
            }

            int yStart = 0;
            int yEnd = boardSize;
            if (dir.Item1 > 0)
            {
                yStart = boardSize - 1;
                yEnd = -1;
            }

            for (int y = yStart; y != yEnd; y -= (dir.Item1 != 0) ? dir.Item1 : 1)
            {
                for (int x = xStart; x != xEnd; x -= (dir.Item2 != 0) ? dir.Item2 : 1)
                {
                    if (board[y, x] == 0)
                    {
                        continue;
                    }

                    int yCheck = y + dir.Item1;
                    int xCheck = x + dir.Item2;

                    while (yCheck >= 0 && yCheck < boardSize && xCheck >= 0 && xCheck < boardSize && board[yCheck, xCheck] == 0)
                    {
                        yCheck += dir.Item1;
                        xCheck += dir.Item2;
                    }

                    // Move back if we went out of bounds
                    if (yCheck < 0 || yCheck >= boardSize || xCheck < 0 || xCheck >= boardSize)
                    {
                        yCheck -= dir.Item1;
                        xCheck -= dir.Item2;
                    }

                    // If no movement, break
                    if (yCheck == y && xCheck == x)
                    {
                        continue;
                    }
                    else if (board[y, x] == board[yCheck, xCheck] && !hadCollision[yCheck, xCheck])
                    {
                        // else If Equal and not combined already, combine
                        hadCollision[yCheck, xCheck] = true;
                        hadMovement = true;
                        board[yCheck, xCheck] += board[y, x];
                        moveScore += board[yCheck, xCheck];
                        board[y, x] = 0;
                    }
                    else if (board[yCheck, xCheck] == 0)
                    {
                        // else if movement into empty tile, simply move
                        hadMovement = true;
                        board[yCheck, xCheck] = board[y, x];
                        board[y, x] = 0;
                    }
                    else
                    {
                        // Else, move back
                        yCheck -= dir.Item1;
                        xCheck -= dir.Item2;
                        if (yCheck == y && xCheck == x)
                        {
                            continue;
                        }
                        hadMovement = true;
                        int temp = board[y, x];
                        board[y, x] = 0;
                        board[yCheck, xCheck] = temp;
                    }
                }
            }
            score += moveScore;
            if (hadMovement && addNextTile)
            {
                AddTile();
            }
            return new Tuple<int, bool>(moveScore, hadMovement);
        }

        // Returns True if no legal moves exist
        public bool CheckLoss()
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (board[y, x] == 0)
                    {
                        return false;
                    }
                    foreach (var dir in directions)
                    {
                        if (y + dir.Item1 >= 0 && y + dir.Item1 < boardSize
                            && x + dir.Item2 >= 0 && x + dir.Item2 < boardSize
                            && board[y, x] == board[y + dir.Item1, x + dir.Item2])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
