using System;
using System.Collections.Generic;
using NoCodeCalculator;
using Xunit;

namespace NoCodeCalculator.Tests
{
    public class NoCodeCostCalculatorTests
    {
        private readonly NoCodeCostCalculator _calculator = new NoCodeCostCalculator();

        #region МЕТОД 1: CalculatePlatformCost

        [Fact]
        public void CalculatePlatformCost_BubbleBaseTariff_ReturnsCorrectPrice()
        {
            // TC01: Розрахунок Bubble (базовий тариф без перевищень)
            // Техніка: Класи еквівалентності (EP), Позитивний тест

            // Arrange (Вхідні дані)
            string platform = "bubble";
            int users = 3;
            double storage = 5.0;

            // Act (Виконання дії)
            double actualCost = _calculator.CalculatePlatformCost(platform, users, storage);

            // Assert (Перевірка результату)
            Assert.Equal(29.0, actualCost);
        }

        [Fact]
        public void CalculatePlatformCost_BubbleBoundaryUsers_ReturnsBasePrice()
        {
            // TC02: Розрахунок Bubble на межі безкоштовних користувачів
            // Техніка: Граничні значення (BVA), Позитивний тест

            // Arrange
            string platform = "bubble";
            int users = 5; // Максимальна межа безкоштовних юзерів
            double storage = 10.0;

            // Act
            double actualCost = _calculator.CalculatePlatformCost(platform, users, storage);

            // Assert
            Assert.Equal(29.0, actualCost);
        }

        [Fact]
        public void CalculatePlatformCost_BubbleAdditionalUser_ChargesExtra()
        {
            // TC03: Розрахунок Bubble з доплатою за 6-го користувача
            // Техніка: Граничні значення (BVA), Позитивний тест

            // Arrange
            string platform = "bubble";
            int users = 6; // На 1 більше за безкоштовний ліміт
            double storage = 10.0;

            // Act
            double actualCost = _calculator.CalculatePlatformCost(platform, users, storage);

            // Assert
            Assert.Equal(39.0, actualCost); // $29 base + $10 за 1 надлишкового юзера
        }

        [Fact]
        public void CalculatePlatformCost_WebflowWithStorageExcess_ChargesExtra()
        {
            // TC04: Розрахунок Webflow з додатковим об'ємом даних
            // Техніка: Класи еквівалентності (EP), Позитивний тест

            // Arrange
            string platform = "webflow";
            int users = 1;
            double storage = 4.0; // Ліміт 2GB, перевищення на 2GB

            // Act
            double actualCost = _calculator.CalculatePlatformCost(platform, users, storage);

            // Assert
            Assert.Equal(24.0, actualCost); // $14 base + 2GB * $5 = $24
        }

        [Fact]
        public void CalculatePlatformCost_NegativeUsers_ThrowsArgumentException()
        {
            // TC05: Валідація: від'ємна кількість користувачів
            // Техніка: Граничні значення (BVA), Негативний тест

            // Arrange
            string platform = "bubble";
            int invalidUsers = -1;
            double storage = 5.0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _calculator.CalculatePlatformCost(platform, invalidUsers, storage));
        }

        [Fact]
        public void CalculatePlatformCost_NegativeStorage_ThrowsArgumentException()
        {
            // TC06: Валідація: від'ємний об'єм бази даних
            // Техніка: Граничні значення (BVA), Негативний тест

            // Arrange
            string platform = "webflow";
            int users = 2;
            double invalidStorage = -2.5;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _calculator.CalculatePlatformCost(platform, users, invalidStorage));
        }

        [Fact]
        public void CalculatePlatformCost_UnknownPlatform_ThrowsKeyNotFoundException()
        {
            // TC07: Валідація: неіснуюча платформа в системі
            // Техніка: Класи еквівалентності (EP), Негативний тест

            // Arrange
            string unknownPlatform = "wordpress";
            int users = 2;
            double storage = 5.0;

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _calculator.CalculatePlatformCost(unknownPlatform, users, storage));
        }

        #endregion

        #region МЕТОД 2: GetCheapestPlatform

        [Fact]
        public void GetCheapestPlatform_SmallProject_ReturnsWebflow()
        {
            // TC08: Пошук найдешевшої платформи для невеликого проєкту
            // Техніка: Класи еквівалентності (EP), Позитивний тест

            // Arrange
            int users = 1;
            double storage = 1.0;

            // Act
            string cheapest = _calculator.GetCheapestPlatform(users, storage);

            // Assert
            Assert.Equal("webflow", cheapest);
        }

        #endregion

        #region МЕТОД 3: CalculateLongTermDiscount

        [Fact]
        public void CalculateLongTermDiscount_12MonthsBoundary_Applies10Percent()
        {
            // TC09: Розрахунок знижки на межі 12 місяців (1 рік, 10%)
            // Техніка: Граничні значення (BVA), Позитивний тест

            // Arrange
            double baseCost = 100.0;
            int contractMonths = 12;

            // Act
            double discountedCost = _calculator.CalculateLongTermDiscount(baseCost, contractMonths);

            // Assert
            Assert.Equal(90.0, discountedCost); // 10% знижки від $100
        }

        [Fact]
        public void CalculateLongTermDiscount_EnterpriseComplexDiscount_AppliesAccumulatedRate()
        {
            // TC10: Розрахунок комплексної знижки для Enterprise (> $500 + 2 роки)
            // Техніка: Класи еквівалентності (EP), Позитивний тест

            // Arrange
            double baseCost = 600.0;
            int contractMonths = 24; // 15% за термін + 5% бонус за Enterprise = 20% разом

            // Act
            double discountedCost = _calculator.CalculateLongTermDiscount(baseCost, contractMonths);

            // Assert
            Assert.Equal(480.0, discountedCost); // Знижка 20% від $600 = $120. Разом: 600 - 120 = 480
        }

        [Fact]
        public void CalculateLongTermDiscount_ContractOver36Months_ThrowsArgumentOutOfRangeException()
        {
            // TC11: Валідація: вихід за межі терміну контракту (> 36 міс)
            // Техніка: Граничні значення (BVA), Негативний тест

            // Arrange
            double baseCost = 100.0;
            int invalidMonths = 40;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _calculator.CalculateLongTermDiscount(baseCost, invalidMonths));
        }

        #endregion
    }
}