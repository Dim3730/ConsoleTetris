using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace ConsoleTetris
{
    class Program
    {
        static List<bool[,]> tetrisFigures = new List<bool[,]>()
        {
            new bool [,]
            {
                {true, true, true, true }
            },
            new bool [,]
            {
                {true, true  },
                {true, true  }
            },
            new bool [,]
            {
                {false, true, false},
                {true, true ,true }
            },
            new bool [,]
            {
                {false, true, true},
                {true, true, false}
            },
            new bool[,]
            {
                {true, true, false},
                {false, true, true}
            },
            new bool[,]
            {
                {false, false, true},
                {true, true, true}
            },
            new bool[,]
            {
                {true, false, false},
                {true, true, true}
            }
        };
        static string figureCharacter = "■";
        static int[] scorePerLines = { 0, 40, 100, 300, 1200 };
        static int tetrisRows = 21;
        static int tetrisCols = 10;
        static int infoCols = 20;
        static int consoleRows = 1 + tetrisRows + 1;
        static int consoleCols = 1 + tetrisCols + 1 + infoCols + 1;
        static int score = 0;
        static int frame = 0;
        static int level = 1;
        static int frameToMoveFigure = 16;
        static int currentFigureRow = 0;
        static int currentFigureCol = 0;
        static int nextFigureRow = 14;
        static int nextFigureCol = tetrisCols + 3;
        static bool[,] currentFigure = null;
        static bool[,] nextFigure = null;
        static bool[,] tetrisField = new bool[tetrisRows, tetrisCols];
        static bool pauseMode = false;
        static bool playGame = true;
        static Random rand = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Тетрис";
            currentFigure = tetrisFigures[rand.Next(0, tetrisFigures.Count)];
            nextFigure = tetrisFigures[rand.Next(0, tetrisFigures.Count)];

            while (true)
            {
                frame++;
                UpdateLevel();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Spacebar && pauseMode == false)
                    {
                        pauseMode = true;

                        Write("╔═══════════════╗", 5, 5);
                        Write("║               ║", 6, 5);
                        Write("║     Pause     ║", 7, 5);
                        Write("║               ║", 8, 5);
                        Write("╚═══════════════╝", 9, 5);
                        playGame = false;
                        Console.ReadKey();
                    }

                    if (key.Key == ConsoleKey.Spacebar && pauseMode == true)
                    {
                        playGame = true;

                        pauseMode = false;
                    }

                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }

                    if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                    {

                        if (currentFigureCol >= 1)
                        {
                            currentFigureCol--;
                        }

                    }

                    if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                    {
                        if ((currentFigureCol < tetrisCols - currentFigure.GetLength(1)))
                        {
                            currentFigureCol++;
                        }
                    }

                    if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                    {
                        RotateCurrentFigure();
                    }

                    if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                    {
                        frame = 1;
                        score += level;
                        currentFigureRow++;
                    }
                }

                if (frame % (frameToMoveFigure - level) == 0)
                {
                    currentFigureRow++;
                    frame = 0;
                    score++;
                }
                if (Collision(currentFigure))
                {
                    AddCurrentFigureToTetrisField();
                    int lines = CheckForFullLines();
                    score += scorePerLines[lines] * level;
                    currentFigureCol = 0;
                    currentFigureRow = 0;

                    if (Collision(currentFigure))
                    {
                        var scoreAsString = score.ToString();
                        scoreAsString += new string(' ', 7 - scoreAsString.Length);
                        Write("╔══════════════╗", 5, 5);
                        Write("║  Game        ║", 6, 5);
                        Write("║     over!    ║", 7, 5);
                        Write($"║      {scoreAsString} ║", 8, 5);
                        Write("╚══════════════╝", 9, 5);
                        playGame = false;
                        Thread.Sleep(1000000);
                        return;
                    }
                }

                DrawBorder();
                DrawInfo();
                DrawTetrisField();
                DrawCurrentFigure();
                Thread.Sleep(40);
            }
        }

        static void Write(string text, int row, int col)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }

        private static bool[,] GetNextFigure()
        {
            nextFigure = tetrisFigures[rand.Next(0, tetrisFigures.Count)];
            return nextFigure;
        }

        private static void RotateCurrentFigure()
        {
            var newFigure = new bool[currentFigure.GetLength(1), currentFigure.GetLength(0)];
            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    newFigure[col, currentFigure.GetLength(0) - row - 1] = currentFigure[row, col];
                }
            }

            if (!Collision(newFigure))
            {
                currentFigure = newFigure;
            }
        }

        private static void AddCurrentFigureToTetrisField()
        {
            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    if (currentFigure[row, col])
                    {
                        tetrisField[currentFigureRow + row, currentFigureCol + col] = true;
                    }
                }
            }
            currentFigure = nextFigure;
            GetNextFigure();
        }

        static bool Collision(bool[,] figure)
        {
            if (currentFigureCol > tetrisCols - figure.GetLength(1))
            {
                return true;
            }
            if (currentFigureRow + figure.GetLength(0) == tetrisRows)
            {
                return true;
            }
            for (int row = 0; row < figure.GetLength(0); row++)
            {
                for (int col = 0; col < figure.GetLength(1); col++)
                {
                    if (figure[row, col] && tetrisField[currentFigureRow + row + 1, currentFigureCol + col])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void DrawTetrisField()
        {
            for (int row = 0; row < tetrisField.GetLength(0); row++)
            {
                string line = "";
                for (int col = 0; col < tetrisField.GetLength(1); col++)
                {
                    if (tetrisField[row, col])
                    {
                        line += $"{figureCharacter}";
                    }
                    else
                    {
                        line += " ";
                    }
                }
                Write(line, row + 1, 1);
            }
        }

        private static int CheckForFullLines()
        {
            int lines = 0;
            for (int row = 0; row < tetrisField.GetLength(0); row++)
            {
                bool rowIsFull = true;
                for (int col = 0; col < tetrisField.GetLength(1); col++)
                {
                    if (!tetrisField[row, col])
                    {
                        rowIsFull = false;
                        break;
                    }
                }
                if (rowIsFull)
                {
                    for (int rowToMove = row; rowToMove >= 1; rowToMove--)
                    {
                        for (int col = 0; col < tetrisField.GetLength(1); col++)
                        {
                            tetrisField[rowToMove, col] = tetrisField[rowToMove - 1, col];
                        }
                    }

                    lines++;
                }
            }
            return lines;
        }

        static void DrawCurrentFigure()
        {
            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    if (currentFigure[row, col])
                    {
                        Write($"{figureCharacter}", row + 1 + currentFigureRow, col + 1 + currentFigureCol);
                    }
                }
            }
        }

        static void DrawNextFigure()
        {
            for (int row = 0; row < nextFigure.GetLength(0); row++)
            {

                for (int col = 0; col < nextFigure.GetLength(1); col++)
                {
                    if (nextFigure[row, col])
                    {
                        Write($"{figureCharacter}", row + 1 + nextFigureRow, col + 1 + nextFigureCol);
                    }
                }
            }
        }

        static void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);

            string firstLine = "╔";
            firstLine += new string('═', tetrisCols);
            firstLine += "╦";
            firstLine += new string('═', infoCols);
            firstLine += "╗";

            string middleLine = "";
            for (int i = 0; i < tetrisRows; i++)
            {
                middleLine += "║";
                middleLine += new string(' ', tetrisCols) + "║" + new string(' ', infoCols) + "║" + "\n";
            }

            string endLine = "╚";
            endLine += new string('═', tetrisCols);
            endLine += "╩";
            endLine += new string('═', infoCols);
            endLine += "╝";

            string borderFrame = firstLine + "\n" + middleLine + endLine;
            Console.Write(borderFrame);
        }

        private static void UpdateLevel()
        {
            if (score <= 0)
            {
                level = 1;
            }
            if (score >= 1000)
            {
                level = 2;
            }
            else if (score == 5000)
            {
                level = 3;
            }
            else if (score == 10000)
            {
                level = 4;
            }
            else if (score == 20000)
            {
                level = 5;
            }
            else if (score == 50000)
            {
                level = 6;
            }
            else if (score == 100000)
            {
                level = 7;
            }
            else if (score == 250000)
            {
                level = 8;
            }
            else if (score == 500000)
            {
                level = 9;
            }
            else if (score == 1000000)
            {
                level = 10;
            }
        }

        static void DrawInfo()
        {
            Write("Счет:", 5, tetrisCols + 3);
            Write(score.ToString(), 7, tetrisCols + 3);
            Write("W - Поворот", 15, tetrisCols + 3);
            Write("A - Влево", 16, tetrisCols + 3);
            Write("D - Вправо", 17, tetrisCols + 3);
            Write("S - Вниз", 18, tetrisCols + 3);
        }
    }
}