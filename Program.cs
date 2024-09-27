using System;
using System.Collections.Generic;

class Program
{
    static char[,] maze;
    static int playerX, playerY;
    static int exitX, exitY;
    static Random random = new Random();

    static void Main()
    {
        int size = 21; // Размер лабиринта (нечетное число)
        InitializeMaze(size);
        GenerateMaze();
        PlacePlayerAndExit();
        PrintMaze();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Q)
            {
                ShowShortestPath();
            }
            else
            {
                MovePlayer(keyInfo.Key);
            }

            if (playerX == exitX && playerY == exitY)
            {
                Console.SetCursorPosition(0, size); // Позиция под лабиринтом
                Console.WriteLine("Поздравляем! Вы вышли из лабиринта!");
                break;
            }
        }
    }

    static void InitializeMaze(int size)
    {
        maze = new char[size, size];

        // Заполняем лабиринт стенами
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                maze[i, j] = '█';
            }
        }
    }

    static void GenerateMaze()
    {
        int startX = 1, startY = 1;
        maze[startX, startY] = ' ';
        RandomWalk(startX, startY);
    }

    static void RandomWalk(int x, int y)
    {
        int[] dx = { 2, -2, 0, 0 };
        int[] dy = { 0, 0, 2, -2 };

        for (int i = 0; i < 4; i++)
        {
            int randIndex = random.Next(4);
            int temp = dx[i];
            dx[i] = dx[randIndex];
            dx[randIndex] = temp;

            temp = dy[i];
            dy[i] = dy[randIndex];
            dy[randIndex] = temp;
        }

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (IsInBounds(newX, newY) && maze[newX, newY] == '█')
            {
                maze[newX, newY] = ' ';
                maze[x + dx[i] / 2, y + dy[i] / 2] = ' ';
                RandomWalk(newX, newY);
            }
        }
    }

    static bool IsInBounds(int x, int y)
    {
        return x > 0 && x < maze.GetLength(0) && y > 0 && y < maze.GetLength(1);
    }

    static void PlacePlayerAndExit()
    {
        playerX = 1;
        playerY = 1;
        maze[playerX, playerY] = 'P';

        exitX = maze.GetLength(0) - 2;
        exitY = maze.GetLength(1) - 2;
        maze[exitX, exitY] = 'E'; // Положение выхода
    }

    static void PrintMaze()
    {
        Console.Clear();
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                Console.Write(maze[i, j]);
            }
            Console.WriteLine();
        }
    }

    static void MovePlayer(ConsoleKey key)
    {
        int oldX = playerX;
        int oldY = playerY;

        if (key == ConsoleKey.W && IsMoveValid(playerX - 1, playerY))
        {
            playerX--; // Вверх
        }
        else if (key == ConsoleKey.S && IsMoveValid(playerX + 1, playerY))
        {
            playerX++; // Вниз
        }
        else if (key == ConsoleKey.A && IsMoveValid(playerX, playerY - 1))
        {
            playerY--; // Влево
        }
        else if (key == ConsoleKey.D && IsMoveValid(playerX, playerY + 1))
        {
            playerY++; // Вправо
        }

        // Обновляем только старую и новую позицию игрока
        Console.SetCursorPosition(oldY, oldX);
        Console.Write(' ');

        Console.SetCursorPosition(playerY, playerX);
        Console.Write('P');
    }

    static bool IsMoveValid(int x, int y)
    {
        return x >= 0 && x < maze.GetLength(0) && y >= 0 && y < maze.GetLength(1) && (maze[x, y] == ' ' || maze[x, y] == 'E');
    }

    // Метод для поиска кратчайшего пути с помощью BFS
    static void ShowShortestPath()
    {
        Queue<(int x, int y)> queue = new Queue<(int, int)>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        (int x, int y)[,] prev = new (int x, int y)[maze.GetLength(0), maze.GetLength(1)]; // Массив для хранения предков

        queue.Enqueue((playerX, playerY));
        visited[playerX, playerY] = true;

        bool found = false;

        // Векторы для смежных перемещений
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        // BFS
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            // Если нашли выход
            if (x == exitX && y == exitY)
            {
                found = true;
                break;
            }

            // По всем возможным направлениям
            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsInBounds(newX, newY) && !visited[newX, newY] && (maze[newX, newY] == ' ' || maze[newX, newY] == 'E'))
                {
                    queue.Enqueue((newX, newY));
                    visited[newX, newY] = true;
                    prev[newX, newY] = (x, y); // Запоминаем, откуда пришли
                }
            }
        }

        if (found)
        {
            // Восстанавливаем путь
            var path = new List<(int x, int y)>();
            int cx = exitX, cy = exitY;

            while (cx != playerX || cy != playerY)
            {
                path.Add((cx, cy));
                (cx, cy) = prev[cx, cy];
            }

            // Выводим путь на экране
            foreach (var (px, py) in path)
            {
                Console.SetCursorPosition(py, px);
                Console.Write('*'); // Отмечаем путь символом '*'
            }

            Console.SetCursorPosition(0, maze.GetLength(0)); // Перемещаем курсор в конец
        }
    }
}
