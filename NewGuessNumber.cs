using System;

namespace GuessNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameStatistics statistics = new GameStatistics();
            GameController gameController = new GameController();

            do
            {
                int attempts = gameController.PlaySingleGame();
                statistics.UpdateStatistics(attempts);

                Console.WriteLine("\nDo you want play another game? (Y/N): ");
            }
            while (Console.ReadLine()?.ToUpper() == "Y");

            statistics.DisplayStatistics();
        }
    }

    /// <summary>
    /// Класс для управления статистикой игр
    /// </summary>
    public class GameStatistics
    {
        private int _minAttempts = int.MaxValue;
        private int _maxAttempts;
        private int _totalAttempts;
        private int _gamesPlayed;

        public void UpdateStatistics(int attempts)
        {
            _gamesPlayed++;
            _totalAttempts += attempts;

            if (attempts < _minAttempts)
                _minAttempts = attempts;

            if (attempts > _maxAttempts)
                _maxAttempts = attempts;
        }

        public void DisplayStatistics()
        {
            if (_gamesPlayed == 0)
            {
                Console.WriteLine("No games were played.");
                return;
            }

            double averageAttempts = (double)_totalAttempts / _gamesPlayed;

            Console.WriteLine($"\n=== Game Statistics ===");
            Console.WriteLine($"Games played: {_gamesPlayed}");
            Console.WriteLine($"Minimum attempts: {_minAttempts}");
            Console.WriteLine($"Maximum attempts: {_maxAttempts}");
            Console.WriteLine($"Average attempts: {averageAttempts:F2}");
            Console.WriteLine($"Total attempts: {_totalAttempts}");
        }
    }

    /// <summary>
    /// Класс для управления игровым процессом
    /// </summary>
    public class GameController
    {
        private readonly Random _random;
        private readonly UserInputHandler _inputHandler;
        private const int MIN_NUMBER = 1;
        private const int MAX_NUMBER = 99;

        public GameController()
        {
            _random = new Random();
            _inputHandler = new UserInputHandler();
        }

        /// <summary>
        /// Проводит одну игру и возвращает количество попыток
        /// </summary>
        public int PlaySingleGame()
        {
            int targetNumber = _random.Next(MIN_NUMBER, MAX_NUMBER + 1);
            int attempts = 0;

            Console.WriteLine($"\n=== New Game ===");
            Console.WriteLine($"Try to guess the number from {MIN_NUMBER} to {MAX_NUMBER}");

            while (true)
            {
                attempts++;
                int userGuess = _inputHandler.GetUserGuess(MIN_NUMBER, MAX_NUMBER);

                GameFeedback feedback = CheckGuess(userGuess, targetNumber);
                Console.WriteLine(feedback.Message);

                if (feedback.IsCorrect)
                {
                    Console.WriteLine($"Congratulations! You won in {attempts} attempts!");
                    return attempts;
                }
            }
        }

        /// <summary>
        /// Проверяет предположение пользователя
        /// </summary>
        private GameFeedback CheckGuess(int guess, int target)
        {
            if (guess == target)
                return new GameFeedback(true, "You are Win!!!");

            string hint = guess > target
                ? "Your number is too high!"
                : "Your number is too low!";

            return new GameFeedback(false, hint);
        }
    }

    /// <summary>
    /// Класс для обратной связи в игре
    /// </summary>
    public class GameFeedback
    {
        public bool IsCorrect { get; }
        public string Message { get; }

        public GameFeedback(bool isCorrect, string message)
        {
            IsCorrect = isCorrect;
            Message = message;
        }
    }

    /// <summary>
    /// Класс для обработки пользовательского ввода
    /// </summary>
    public class UserInputHandler
    {
        private const int MAX_INPUT_ATTEMPTS = 3;

        /// <summary>
        /// Получает корректное число от пользователя
        /// </summary>
        public int GetUserGuess(int minValue, int maxValue)
        {
            for (int attempt = 0; attempt < MAX_INPUT_ATTEMPTS; attempt++)
            {
                Console.Write($"Enter your guess ({minValue}-{maxValue}): ");
                string input = Console.ReadLine();

                if (TryParseGuess(input, minValue, maxValue, out int guess))
                    return guess;

                Console.WriteLine($"Invalid input! Please enter a number between {minValue} and {maxValue}.");

                if (attempt == MAX_INPUT_ATTEMPTS - 1)
                {
                    Console.WriteLine("Too many invalid attempts. Exiting program...");
                    Environment.Exit(1);
                }
            }

            return 0; // Эта строка никогда не выполнится
        }

        /// <summary>
        /// Пытается преобразовать строку в число и проверить его в диапазоне
        /// </summary>
        private bool TryParseGuess(string input, int minValue, int maxValue, out int result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (!int.TryParse(input.Trim(), out int number))
                return false;

            if (number < minValue || number > maxValue)
                return false;

            result = number;
            return true;
        }
    }
}