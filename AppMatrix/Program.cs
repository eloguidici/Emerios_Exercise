using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

public class Program
{
    const string FOLDER = "files/";
    const string HORIZONTAL_SEQUENCE_MESSAGE = "The longest sequence is {0} and is a horizontal sequence.";
    const string VERTICAL_SEQUENCE_MESSAGE = "The longest sequence is {0} and is a vertical sequence.";
    const string DIAGONAL_LEFT_TO_RIGTH_SEQUENCE_MESSAGE = "The longest sequence is {0} and is a diagonal sequence from left to right.";
    const string DIAGONAL_RIGTH_TO_LEFT_SEQUENCE_MESSAGE = "The longest sequence is {0} and is a diagonal sequence from right to left.";

    static readonly List<Tuple<int, int>> _horizontalPositions = new();
    static readonly List<Tuple<int, int>> _verticalPositions = new();
    static readonly List<Tuple<int, int>> _diagonalRigthToLeftPositions = new();
    static readonly List<Tuple<int, int>> _diagonalLeftToRigthPositions = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        int min = 4;
        int max = 8;

        Console.WriteLine("Welcome!");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine();
            Console.WriteLine("1 - Find the longest sequence in a random matrix.");
            Console.WriteLine("2 - Find the longest sequence in an matrix of an existing file in the project.");
            Console.WriteLine("3 - Find the longest sequence in an matrix entered by console.");
            Console.WriteLine("4 - Exit");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Clear();

                    Console.WriteLine($"Enter the dimensions of the square matrix (min {min}, max {max}):");

                    string dimension = Console.ReadLine();

                    if (!int.TryParse(dimension, out int result) || (result < min || result > max))
                    {
                        Console.WriteLine($"The dimension is invalid.");
                        Console.WriteLine("Shall We continue...?");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }

                    Console.WriteLine();

                    char[,] matrix = GenerateRandomMatrix(result);
                    Console.WriteLine("Randomly generated matrix:");

                    Process(matrix);

                    break;
                case "2":
                    Console.Clear();

                    PrintFiles();

                    Console.WriteLine();
                    Console.WriteLine("Choose one of these existing files (including the extension):");

                    string fileName = Console.ReadLine();

                    string path = FOLDER + fileName;
                    if (!File.Exists(path))
                    {
                        Console.WriteLine($"The file {fileName} does not exist. Please enter a valid file name.");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }
                    Console.WriteLine();

                    char[,] loadedMatrix = ReadMatrixFromFile(path);
                    Console.WriteLine("Matrix loaded from file:");

                    Process(loadedMatrix);
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Enter the values of the square matrix separated by commas (Ex: B, B, D, A, D, E, 3, B, X)");
                    Console.WriteLine("In cases where a non-alphanumeric character has been entered, it will be replaced by a random one.");

                    string input = Console.ReadLine();

                    var isValid = IsValidMatrix(input);
                    if (!isValid)
                    {
                        Console.WriteLine("The number of elements entered does not correspond to a square matrix");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }

                    char[,] consoleMatrix = ReadMatrixFromInputConsole(input.ToUpper());
                    Console.WriteLine("Matrix loaded from input console:");

                    Process(consoleMatrix);
                    break;
                case "4":
                    Console.Clear();
                    Console.WriteLine("Thank you!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option, please select a valid option.");
                    Console.ReadLine();
                    Console.Clear();
                    break;
            }
        }
    }

    /// <summary>
    /// This method prints the name of all files in the folder specified by the FOLDER constant.
    /// </summary>
    private static void PrintFiles()
    {
        var files = Directory.GetFiles(Environment.CurrentDirectory + "\\" + FOLDER);
        foreach (var file in files)
        {
            Console.WriteLine($"{Path.GetFileName(file)}");
        }
    }

    /// <summary>
    /// This method processes the given matrix and finds the longest sequence horizontally, vertically, and diagonally.
    /// </summary>
    /// <param name="matrix"></param>
    static void Process(char[,] matrix)
    {

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int maxLength = 0;
        string longestSequence = string.Empty;
        string longestSequenceMessage = String.Empty;
        List<Tuple<int, int>> positions = new();

        Console.WriteLine();

        // find the longest horizontal sequence
        string sequenceHorizontal = SequenceHorizontal(matrix, rows, cols);
        if (sequenceHorizontal.Length > maxLength)
        {
            maxLength = sequenceHorizontal.Length;
            longestSequence = sequenceHorizontal;
            longestSequenceMessage = HORIZONTAL_SEQUENCE_MESSAGE;
            positions = _horizontalPositions;
        }

        // find the longest vertical sequence
        string sequenceVertical = SequenceVertical(matrix, rows, cols);
        if (sequenceVertical.Length > maxLength)
        {
            maxLength = sequenceVertical.Length;
            longestSequence = sequenceVertical;
            longestSequenceMessage = VERTICAL_SEQUENCE_MESSAGE;
            positions = _verticalPositions;
        }

        // find the longest diagonal sequence
        string sequenceDiagonalLeftToRight = SequenceDiagonalLeftToRight(matrix, rows, cols);
        if (sequenceDiagonalLeftToRight.Length > maxLength)
        {
            maxLength = sequenceDiagonalLeftToRight.Length;
            longestSequence = sequenceDiagonalLeftToRight;
            longestSequenceMessage = DIAGONAL_LEFT_TO_RIGTH_SEQUENCE_MESSAGE;
            positions = _diagonalLeftToRigthPositions;
        }

        // find the longest diagonal sequence
        string sequenceDiagonalRightToLeft = SequenceDiagonalRightToLeft(matrix, rows, cols);
        if (sequenceDiagonalRightToLeft.Length > maxLength)
        {
            longestSequence = sequenceDiagonalRightToLeft;
            longestSequenceMessage = DIAGONAL_RIGTH_TO_LEFT_SEQUENCE_MESSAGE;
            positions = _diagonalRigthToLeftPositions;
        }
        PrintColorMatrix(positions, matrix);

        Console.WriteLine();
        Console.WriteLine(string.Format(longestSequenceMessage, longestSequence));
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Shall We continue...?");
        Console.ReadLine();
        Console.Clear();
    }

    /// <summary>
    /// This method checks if the given input string can be parsed into a square matrix.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool IsValidMatrix(string input)
    {
        if (string.IsNullOrEmpty(input.Trim())) return false;
        if (Regex.IsMatch(input, @"^(\w,)*\w$")) return false;
        string[] subcadenas = input.Split(',');
        int elementos = subcadenas.Length;
        double raiz = Math.Sqrt(elementos);
        return (raiz == (int)raiz);
    }


    /// <summary>
    /// This method converts the given input string into a char[,] array that represents a square matrix.
    /// In cases where a non-alphanumeric character has been entered, it will be replaced by a random one.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    static char[,] ReadMatrixFromInputConsole(string input)
    {
        string output = string.Concat(input.Select(c => (char.IsLetterOrDigit(c) || c == ',') ? c : GetRandomChar()));
        string[] characters = output.Split(',');
        int elements = characters.Length;
        double sqrt = Math.Sqrt(elements);
        int dimension = (int)sqrt;
        char[,] matrix = new char[dimension, dimension];
        int index = 0;
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                matrix[row, col] = characters[index++].Trim()[0];
            }
        }
        return matrix;
    }

    /// <summary>
    /// Gets a random alphanumeric character.
    /// </summary>
    /// <returns></returns>
    private static char GetRandomChar()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new();
        var value = chars[random.Next(chars.Length)];
        return value;
    }

    /// <summary>
    /// This method reads a char[,] matrix from a file with the given path.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    static char[,] ReadMatrixFromFile(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);
        int numRows = lines.Length;
        int numCols = lines[0].Split(',').Length;
        char[,] matrix = new char[numRows, numCols];
        for (int row = 0; row < numRows; row++)
        {
            string[] rowValues = lines[row].Split(',');
            for (int col = 0; col < numCols; col++)
            {
                matrix[row, col] = rowValues[col].Trim()[0];
            }
        }
        return matrix;
    }

    /// <summary>
    /// This method finds the longest sequence in the given char[,] matrix that goes horizontally.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    static string SequenceHorizontal(char[,] matrix, int rows, int cols)
    {
        _horizontalPositions.Clear();
        string longestSequence = string.Empty;
        var longestPositions = new List<Tuple<int, int>>();

        for (int row = 0; row < rows; row++)
        {
            string currentSequence = string.Empty;
            var currentPositions = new List<Tuple<int, int>>();

            currentSequence += matrix[row, 0];
            currentPositions.Add(Tuple.Create(row, 0));
            for (int col = 1; col < cols; col++)
            {
                if (matrix[row, col] == matrix[row, col - 1])
                {
                    currentSequence += matrix[row, col];
                    currentPositions.Add(Tuple.Create(row, col));
                }
                else
                {
                    if (currentSequence.Length > longestSequence.Length)
                    {
                        longestSequence = currentSequence;
                        longestPositions = currentPositions;
                    }
                    currentSequence = string.Empty + matrix[row, col];
                    currentPositions = new List<Tuple<int, int>>
                    {
                        Tuple.Create(row, col)
                    };
                }
            }

            if (currentSequence.Length > longestSequence.Length)
            {
                longestSequence = currentSequence;
                longestPositions = currentPositions;
            }
        }
        _horizontalPositions.AddRange(longestPositions);
        return longestSequence;
    }

    /// <summary>
    /// This method finds the longest sequence in the given char[,] matrix that goes vertically.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    static string SequenceVertical(char[,] matrix, int rows, int cols)
    {
        _verticalPositions.Clear();
        string longestSequence = string.Empty;
        var longestPositions = new List<Tuple<int, int>>();

        for (int col = 0; col < cols; col++)
        {
            string currentSequence = string.Empty;
            var currentPositions = new List<Tuple<int, int>>();

            currentSequence += matrix[0, col];
            currentPositions.Add(Tuple.Create(0, col));

            for (int row = 1; row < rows; row++)
            {
                if (matrix[row, col] == matrix[row - 1, col])
                {
                    currentSequence += matrix[row, col];
                    currentPositions.Add(Tuple.Create(row, col));
                }
                else
                {
                    if (currentSequence.Length > longestSequence.Length)
                    {
                        longestSequence = currentSequence;
                        longestPositions = currentPositions;
                    }
                    currentSequence = string.Empty + matrix[row, col];
                    currentPositions = new List<Tuple<int, int>>
                    {
                        Tuple.Create(row, col)
                    };
                }
            }
            if (currentSequence.Length > longestSequence.Length)
            {
                longestSequence = currentSequence;
                longestPositions = currentPositions;
            }
        }
        _verticalPositions.AddRange(longestPositions);
        return longestSequence;
    }

    /// <summary>
    /// This method finds the longest sequence in the given char[,] matrix that goes from left to right diagonally.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    static string SequenceDiagonalLeftToRight(char[,] matrix, int rows, int cols)
    {
        _diagonalLeftToRigthPositions.Clear();
        string longestSequence = string.Empty;
        var longestPositions = new List<Tuple<int, int>>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                string currentSequence = string.Empty;
                var currentPositions = new List<Tuple<int, int>>();
                int x = row;
                int y = col;
                while (x < rows && y < cols)
                {
                    if (x == row && y == col)
                    {
                        currentSequence += matrix[x, y];
                        currentPositions.Add(Tuple.Create(x, y));
                    }
                    else
                    {
                        if (matrix[x, y] == matrix[x - 1, y - 1])
                        {
                            currentSequence += matrix[x, y];
                            currentPositions.Add(Tuple.Create(x, y));
                        }
                        else
                        {
                            if (currentSequence.Length > longestSequence.Length)
                            {
                                longestSequence = currentSequence;
                                longestPositions = currentPositions;
                            }
                            currentSequence = string.Empty + matrix[x, y];
                            currentPositions = new List<Tuple<int, int>>
                                                {
                                                    Tuple.Create(x, y)
                                                };
                        }
                    }
                    x++;
                    y++;
                }

                if (currentSequence.Length > longestSequence.Length)
                {
                    longestSequence = currentSequence;
                    longestPositions = currentPositions;
                }
            }
        }
        _diagonalLeftToRigthPositions.AddRange(longestPositions);
        return longestSequence;
    }

    /// <summary>
    /// This method finds the longest sequence in the given char[,] matrix that goes from right to left diagonally.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    static string SequenceDiagonalRightToLeft(char[,] matrix, int rows, int cols)
    {
        _diagonalRigthToLeftPositions.Clear();
        string longestSequence = string.Empty;
        var longestPositions = new List<Tuple<int, int>>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = cols - 1; col >= 0; col--)
            {
                string currentSequence = string.Empty;
                var currentPositions = new List<Tuple<int, int>>();
                int x = row;
                int y = col;
                while (x < rows && y >= 0)
                {
                    if (x == row && y == col)
                    {
                        currentSequence += matrix[x, y];
                        currentPositions.Add(Tuple.Create(x, y));
                    }
                    else
                    {
                        if (matrix[x, y] == matrix[x - 1, y + 1])
                        {
                            currentSequence += matrix[x, y];
                            currentPositions.Add(Tuple.Create(x, y));
                        }
                        else
                        {
                            if (currentSequence.Length > longestSequence.Length)
                            {
                                longestSequence = currentSequence;
                                longestPositions = currentPositions;
                            }
                            currentSequence = string.Empty + matrix[x, y];
                            currentPositions = new List<Tuple<int, int>>
                                                {
                                                    Tuple.Create(x, y)
                                                };
                        }
                    }
                    x++;
                    y--;
                }
                if (currentSequence.Length > longestSequence.Length)
                {
                    longestSequence = currentSequence;
                    longestPositions = currentPositions;
                }
            }
        }
        _diagonalRigthToLeftPositions.AddRange(longestPositions);
        return longestSequence;
    }

    /// <summary>
    /// This method generates a random char[,] array that represents a square matrix with the given dimension.
    /// </summary>
    /// <param name="dimension"></param>
    /// <returns></returns>
    static char[,] GenerateRandomMatrix(int dimension)
    {
        const string chars = "ABC123";
        var random = new Random();
        var matrix = new char[dimension, dimension];

        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                matrix[row, col] = chars[random.Next(chars.Length)];
            }
        }

        return matrix;
    }

    /// <summary>
    /// Prints a matrix of characters to the console, with certain elements in red based on their position. 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="matrix"></param>
    public static void PrintColorMatrix(List<Tuple<int, int>> positions, char[,] matrix)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                bool found = false;
                foreach (var position in positions)
                {
                    if (position.Item1 == row && position.Item2 == col)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Console.ResetColor();
                }
                Console.Write("{0,8}", matrix[row, col]);
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}
