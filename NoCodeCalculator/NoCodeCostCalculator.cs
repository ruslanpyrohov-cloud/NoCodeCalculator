using System;
using System.Collections.Generic;

namespace NoCodeCalculator
{
    /// <summary>
    /// Головний модуль розрахунку вартості розробки та підтримки no-code проєктів.
    /// </summary>
    public class NoCodeCostCalculator
    {
        // Базова база даних платформ та їх тарифів для розрахунків
        private readonly Dictionary<string, (double BasePrice, double PricePerUser, double IncludedGb, double PricePerGb)> _platforms =
            new Dictionary<string, (double, double, double, double)>(StringComparer.OrdinalIgnoreCase)
            {
                { "bubble", (29.0, 10.0, 10.0, 2.0) },       // Base $29, >5 користувачів: +$10/користувач, 10GB включено, +$2/GB
                { "webflow", (14.0, 19.0, 2.0, 5.0) },       // Base $14, кожен додатковий користувач +$19, 2GB включено, +$5/GB
                { "flutterflow", (30.0, 25.0, 20.0, 1.0) }   // Base $30, кожен користувач +$25, 20GB включено, +$1/GB
            };

        /// <summary>
        /// МЕТОД 1: Розрахунок місячної вартості для конкретної платформи з валідацією та обробкою винятків.
        /// </summary>
        public double CalculatePlatformCost(string platformName, int users, double dataSizeGb)
        {
            if (string.IsNullOrWhiteSpace(platformName))
                throw new ArgumentNullException(nameof(platformName), "Назва платформи не може бути порожньою.");

            if (users < 0)
                throw new ArgumentException("Кількість користувачів не може бути від’ємною.", nameof(users));

            if (dataSizeGb < 0)
                throw new ArgumentException("Об’єм бази даних не може бути від’ємним.", nameof(dataSizeGb));

            if (!_platforms.ContainsKey(platformName))
                throw new KeyNotFoundException($"Платформу '{platformName}' не знайдено в базі даних системних тарифів.");

            var tariff = _platforms[platformName];
            double basePrice = tariff.BasePrice;
            double userCost = 0;

            // Логіка розрахунку користувачів залежно від специфіки платформи
            if (platformName.Equals("bubble", StringComparison.OrdinalIgnoreCase))
            {
                if (users > 5)
                {
                    userCost = (users - 5) * tariff.PricePerUser;
                }
            }
            else
            {
                if (users > 1)
                {
                    userCost = (users - 1) * tariff.PricePerUser;
                }
            }

            // Розрахунок перевищення ліміту дискового простору
            double storageCost = 0;
            if (dataSizeGb > tariff.IncludedGb)
            {
                storageCost = (dataSizeGb - tariff.IncludedGb) * tariff.PricePerGb;
            }

            return basePrice + userCost + storageCost;
        }

        /// <summary>
        /// МЕТОД 2: Пошук найоптимальнішої (найдешевшої) платформи за допомогою циклу.
        /// </summary>
        public string GetCheapestPlatform(int users, double dataSizeGb)
        {
            if (users < 0 || dataSizeGb < 0)
                throw new ArgumentException("Вхідні параметри проєкту для аналізу порівняння є некоректними.");

            string cheapestPlatform = string.Empty;
            double minCost = double.MaxValue;

            foreach (var platform in _platforms.Keys)
            {
                try
                {
                    double currentCost = CalculatePlatformCost(platform, users, dataSizeGb);

                    if (currentCost < minCost)
                    {
                        minCost = currentCost;
                        cheapestPlatform = platform;
                    }
                }
                catch (Exception)
                {
                    // Ігноруємо помилки всередині циклу порівняння
                }
            }

            if (string.IsNullOrEmpty(cheapestPlatform))
                throw new InvalidOperationException("Неможливо визначити найдешевшу платформу для вказаної конфігурації.");

            return cheapestPlatform;
        }

        /// <summary>
        /// МЕТОД 3: Розрахунок довгострокової накопичувальної знижки (багаторівнева умовна конструкція).
        /// </summary>
        public double CalculateLongTermDiscount(double baseCost, int contractMonths)
        {
            if (baseCost < 0)
                throw new ArgumentException("Базова вартість проєкту не може быть від’ємною.");

            if (contractMonths <= 0 || contractMonths > 36)
                throw new ArgumentOutOfRangeException(nameof(contractMonths), "Термін тривалості контракту має бути в межах від 1 до 36 місяців.");

            double discountPercentage = 0;

            if (contractMonths >= 12 && contractMonths < 24)
            {
                discountPercentage = 0.10;
            }
            else if (contractMonths >= 24 && contractMonths < 36)
            {
                discountPercentage = 0.15;
            }
            else if (contractMonths == 36)
            {
                discountPercentage = 0.20;
            }

            if (baseCost > 500.0)
            {
                discountPercentage += 0.05;
            }

            double totalDiscount = baseCost * discountPercentage;
            return baseCost - totalDiscount;
        }
    }
}