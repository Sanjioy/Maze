using System;

class Program
{
    ///<summary>Двумерный массив для представления лабиринта.</summary>
    static char[,] maze;

    ///<summary>Координаты игрока по оси X.</summary>
    static int playerX, playerY;

    ///<summary>Координаты выхода по оси X.</summary>
    static int exitX, exitY;

    ///<summary>Объект для генерации случайных чисел.</summary>
    static Random random = new Random();

    static void Main()
    {
        ///<summary>Размер лабиринта (нечетное число).</summary>
        int size = 21;
        InitializeMaze(size);
        GenerateMaze();
        PlacePlayerAndExit();
        PrintMaze();

        ///<summary>Игровой цикл, ожидающий ввода от пользователя.</summary>
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

            ///<summary>Проверка на достижение выхода.</summary>
            if (playerX == exitX && playerY == exitY)
            {
                Console.SetCursorPosition(0, size);
                Console.WriteLine("Поздравляем! Вы вышли из лабиринта!");
                break;
            }
        }
    }

    ///<summary>Инициализация лабиринта, заполнение его стенами.</summary>
    static void InitializeMaze(int size)
    {
        maze = new char[size, size];

        ///<summary>Заполнение лабиринта стенами.</summary>
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                maze[i, j] = '█';
            }
        }
    }

    ///<summary>Генерация лабиринта с помощью случайной прогулки.</summary>
    static void GenerateMaze()
    {
        int startX = 1, startY = 1;
        maze[startX, startY] = ' ';
        RandomWalk(startX, startY);
    }

    ///<summary>Рекурсивный метод для реализации случайной прогулки.</summary>
    static void RandomWalk(int x, int y)
    {
        int[] dx = { 2, -2, 0, 0 };
        int[] dy = { 0, 0, 2, -2 };

        ///<summary>Перемешивание направлений для случайного выбора движения.</summary>
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

        ///<summary>Проверка новых координат и обновление лабиринта.</summary>
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

    ///<summary>Проверка, находится ли позиция в пределах лабиринта.</summary>
    static bool IsInBounds(int x, int y)
    {
        return x > 0 && x < maze.GetLength(0) && y > 0 && y < maze.GetLength(1);
    }

    ///<summary>Размещение игрока и выхода в лабиринте.</summary>
    static void PlacePlayerAndExit()
    {
        playerX = 1;
        playerY = 1;
        maze[playerX, playerY] = 'P';

        exitX = maze.GetLength(0) - 2;
        exitY = maze.GetLength(1) - 2;
        maze[exitX, exitY] = 'E';
    }

    ///<summary>Вывод лабиринта на экран.</summary>
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

    ///<summary>Перемещение игрока в соответствии с нажатой клавишей.</summary>
    static void MovePlayer(ConsoleKey key)
    {
        int oldX = playerX;
        int oldY = playerY;

        if (key == ConsoleKey.W && IsMoveValid(playerX - 1, playerY))
        {
            playerX--;
        }
        else if (key == ConsoleKey.S && IsMoveValid(playerX + 1, playerY))
        {
            playerX++;
        }
        else if (key == ConsoleKey.A && IsMoveValid(playerX, playerY - 1))
        {
            playerY--;
        }
        else if (key == ConsoleKey.D && IsMoveValid(playerX, playerY + 1))
        {
            playerY++;
        }

        ///<summary>Обновление позиции игрока на экране.</summary>
        Console.SetCursorPosition(oldY, oldX);
        Console.Write(' ');

        Console.SetCursorPosition(playerY, playerX);
        Console.Write('P');
    }

    ///<summary>Проверка допустимости движения игрока.</summary>
    static bool IsMoveValid(int x, int y)
    {
        return x >= 0 && x < maze.GetLength(0) && y >= 0 && y < maze.GetLength(1) && (maze[x, y] == ' ' || maze[x, y] == 'E');
    }

    ///<summary>Метод для поиска кратчайшего пути с помощью алгоритма BFS.</summary>
    static void ShowShortestPath()
    {
        Queue<(int x, int y)> queue = new Queue<(int, int)>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        (int x, int y)[,] prev = new (int x, int y)[maze.GetLength(0), maze.GetLength(1)];

        queue.Enqueue((playerX, playerY));
        visited[playerX, playerY] = true;

        bool found = false;

        ///<summary>Векторы для смежных перемещений.</summary>
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        ///<summary>Алгоритм BFS для поиска кратчайшего пути.</summary>
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            ///<summary>Проверка на достижение выхода.</summary>
            if (x == exitX && y == exitY)
            {
                found = true;
                break;
            }

            ///<summary>Поиск всех возможных направлений.</summary>
            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsInBounds(newX, newY) && !visited[newX, newY] && (maze[newX, newY] == ' ' || maze[newX, newY] == 'E'))
                {
                    queue.Enqueue((newX, newY));
                    visited[newX, newY] = true;
                    prev[newX, newY] = (x, y);
                }
            }
        }

        ///<summary>Восстановление и вывод кратчайшего пути на экран.</summary>
        if (found)
        {
            var path = new List<(int x, int y)>();
            int cx = exitX, cy = exitY;

            while (cx != playerX || cy != playerY)
            {
                path.Add((cx, cy));
                (cx, cy) = prev[cx, cy];
            }

            ///<summary>Отметка пути символом '*'.</summary>
            foreach (var (px, py) in path)
            {
                Console.SetCursorPosition(py, px);
                Console.Write('*');
            }

            Console.SetCursorPosition(0, maze.GetLength(0));
        }
    }
}
