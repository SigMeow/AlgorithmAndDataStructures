using System;

namespace GuessNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создание объектов для статистики и управления игрой
            GameStatistics statistics = new GameStatistics();
            GameController gameController = new GameController();
            
            do
            {
                // Запуск одной игры и получение количества попыток
                int attempts = gameController.PlaySingleGame();
                // Обновление статистики после игры
                statistics.UpdateStatistics(attempts);
                
                Console.WriteLine("\nDo you want play another game? (Y/N): ");
            }
            while (Console.ReadLine()?.ToUpper() == "Y"); // Проверка желания продолжить
            
            // Вывод итоговой статистики после всех игр
            statistics.DisplayStatistics();
        }
    }

    /// <summary>
    /// Класс для управления статистикой игр
    /// </summary>
    public class GameStatistics
    {
        // Поля для хранения статистических данных
        private int _minAttempts = int.MaxValue; // Минимальное количество попыток
        private int _maxAttempts; // Максимальное количество попыток
        private int _totalAttempts; // Сумма всех попыток
        private int _gamesPlayed; // Количество сыгранных игр

        /// <summary>
        /// Обновление статистики после каждой игры
        /// </summary>
        public void UpdateStatistics(int attempts)
        {
            _gamesPlayed++; // Увеличиваем счетчик игр
            _totalAttempts += attempts; // Добавляем попытки к общей сумме
            
            // Обновление минимума
            if (attempts < _minAttempts)
                _minAttempts = attempts;
                
            // Обновление максимума    
            if (attempts > _maxAttempts)
                _maxAttempts = attempts;
        }

        /// <summary>
        /// Вывод статистики в консоль
        /// </summary>
        public void DisplayStatistics()
        {
            // Проверка на наличие сыгранных игр
            if (_gamesPlayed == 0)
            {
                Console.WriteLine("No games were played.");
                return;
            }

            // Вычисление среднего количества попыток
            double averageAttempts = (double)_totalAttempts / _gamesPlayed;
            
            // Вывод всей статистики
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
        // Зависимости для работы игры
        private readonly Random _random; // Генератор случайных чисел
        private readonly UserInputHandler _inputHandler; // Обработчик ввода
        private const int MIN_NUMBER = 1; // Минимальное число
        private const int MAX_NUMBER = 99; // Максимальное число

        public GameController()
        {
            // Инициализация зависимостей
            _random = new Random();
            _inputHandler = new UserInputHandler();
        }

        /// <summary>
        /// Проводит одну игру и возвращает количество попыток
        /// </summary>
        public int PlaySingleGame()
        {
            // Генерация случайного числа для угадывания
            int targetNumber = _random.Next(MIN_NUMBER, MAX_NUMBER + 1);
            int attempts = 0; // Счетчик попыток
            
            Console.WriteLine($"\n=== New Game ===");
            Console.WriteLine($"Try to guess the number from {MIN_NUMBER} to {MAX_NUMBER}");
            
            // Основной игровой цикл
            while (true)
            {
                attempts++; // Увеличиваем счетчик попыток
                // Получение числа от пользователя
                int userGuess = _inputHandler.GetUserGuess(MIN_NUMBER, MAX_NUMBER);
                
                // Проверка предположения
                GameFeedback feedback = CheckGuess(userGuess, targetNumber);
                Console.WriteLine(feedback.Message); // Вывод подсказки
                
                // Проверка на победу
                if (feedback.IsCorrect)
                {
                    Console.WriteLine($"Congratulations! You won in {attempts} attempts!");
                    return attempts; // Возвращаем количество попыток
                }
            }
        }

        /// <summary>
        /// Проверяет предположение пользователя
        /// </summary>
        private GameFeedback CheckGuess(int guess, int target)
        {
            // Проверка на правильное угадывание
            if (guess == target)
                return new GameFeedback(true, "You are Win!!!");
            
            // Формирование подсказки (выше или ниже)
            string hint = guess > target 
                ? "Your number is too high!" // Если число больше загаданного
                : "Your number is too low!"; // Если число меньше загаданного
                
            return new GameFeedback(false, hint);
        }
    }

    /// <summary>
    /// Класс для обратной связи в игре
    /// </summary>
    public class GameFeedback
    {
        // Свойства только для чтения
        public bool IsCorrect { get; } // Флаг правильности ответа
        public string Message { get; } // Сообщение пользователю

        public GameFeedback(bool isCorrect, string message)
        {
            // Инициализация через конструктор
            IsCorrect = isCorrect;
            Message = message;
        }
    }

    /// <summary>
    /// Класс для обработки пользовательского ввода
    /// </summary>
    public class UserInputHandler
    {
        private const int MAX_INPUT_ATTEMPTS = 3; // Максимум попыток ввода

        /// <summary>
        /// Получает корректное число от пользователя
        /// </summary>
        public int GetUserGuess(int minValue, int maxValue)
        {
            // Цикл с ограниченным количеством попыток ввода
            for (int attempt = 0; attempt < MAX_INPUT_ATTEMPTS; attempt++)
            {
                Console.Write($"Enter your guess ({minValue}-{maxValue}): ");
                string input = Console.ReadLine();
                
                // Попытка распарсить ввод
                if (TryParseGuess(input, minValue, maxValue, out int guess))
                    return guess; // Успешное получение числа
                
                Console.WriteLine($"Invalid input! Please enter a number between {minValue} and {maxValue}.");
                
                // Проверка на последнюю попытку
                if (attempt == MAX_INPUT_ATTEMPTS - 1)
                {
                    Console.WriteLine("Too many invalid attempts. Exiting program...");
                    Environment.Exit(1); // Аварийное завершение программы
                }
            }
            
            return 0; // Никогда не выполнится (нужно для компиляции)
        }

        /// <summary>
        /// Пытается преобразовать строку в число и проверить его в диапазоне
        /// </summary>
        private bool TryParseGuess(string input, int minValue, int maxValue, out int result)
        {
            result = 0; // Инициализация выходного параметра
            
            // Проверка на пустой ввод
            if (string.IsNullOrWhiteSpace(input))
                return false;
                
            // Попытка преобразования в число
            if (!int.TryParse(input.Trim(), out int number))
                return false;
                
            // Проверка нахождения в диапазоне
            if (number < minValue || number > maxValue)
                return false;
                
            result = number; // Сохраняем результат
            return true; // Успешное преобразование
        }
    }
}
