using System;
using System.Text;

namespace NoCodeCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Эта строчка принудительно включает поддержку UTF-8 в консоли
            // Она полностью уберет знаки вопроса вместо букв "і" и "є"
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Веб-калькулятор no-code проєктів ===");
            Console.WriteLine("Модуль бізнес-логіки успішно зібрано.");
            Console.WriteLine("Основна перевірка коду буде виконуватися через Unit-тести.");

            Console.ReadKey();
        }
    }
}