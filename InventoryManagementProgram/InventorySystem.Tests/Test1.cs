using InventorySystem.API.Helpers;

namespace InventorySystem.Tests
{
    [TestClass]
    public sealed class ConversionHelperTests
    {
        //Convert-metoden

        [TestMethod]
        public void Convert_KgToLiter_ReturnsCorrectValue()
        {
            // 5 kg mjölk, faktor 0.971 (1 kg mjölk ≈ 0.971 liter)
            double result = ConversionHelper.Convert(5.0, 0.971);
            Assert.AreEqual(4.85, result);
        }

        [TestMethod]
        public void Convert_KgToStyck_ReturnsCorrectValue()
        {
            // 3 kg ägg, faktor 16.67 (1 kg ≈ 16.67 ägg)
            double result = ConversionHelper.Convert(3.0, 16.67);
            Assert.AreEqual(50.01, result);
        }

        [TestMethod]
        public void Convert_KgToKg_ReturnsSameValue()
        {
            // Faktor 1.0 = ingen omvandling
            double result = ConversionHelper.Convert(2.5, 1.0);
            Assert.AreEqual(2.5, result);
        }

        [TestMethod]
        public void Convert_ZeroWeight_ReturnsZero()
        {
            double result = ConversionHelper.Convert(0, 0.971);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Convert_NegativeWeight_ReturnsZero()
        {
            double result = ConversionHelper.Convert(-1.0, 0.971);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Convert_RoundsToTwoDecimals()
        {
            // 1.555 * 1.0 = 1.555 → ska avrundas till 1.56
            double result = ConversionHelper.Convert(1.555, 1.0);
            Assert.AreEqual(1.56, result);
        }

        [TestMethod]
        public void Convert_LargeWeight_ReturnsCorrectValue()
        {
            // 100 kg med faktor 2.5
            double result = ConversionHelper.Convert(100.0, 2.5);
            Assert.AreEqual(250.0, result);
        }

        [TestMethod]
        public void Convert_SmallFactor_ReturnsCorrectValue()
        {
            // 10 kg med faktor 0.01
            double result = ConversionHelper.Convert(10.0, 0.01);
            Assert.AreEqual(0.1, result);
        }

        // SumConverted-metoden 

        [TestMethod]
        public void SumConverted_MultipleValues_ReturnsSum()
        {
            // Tre vågar med mjölk: 4.86 + 2.91 + 1.94 = 9.71
            var values = new List<double> { 4.86, 2.91, 1.94 };
            double result = ConversionHelper.SumConverted(values);
            Assert.AreEqual(9.71, result);
        }

        [TestMethod]
        public void SumConverted_SingleValue_ReturnsThatValue()
        {
            var values = new List<double> { 4.86 };
            double result = ConversionHelper.SumConverted(values);
            Assert.AreEqual(4.86, result);
        }

        [TestMethod]
        public void SumConverted_EmptyList_ReturnsZero()
        {
            var values = new List<double>();
            double result = ConversionHelper.SumConverted(values);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void SumConverted_RoundsResult()
        {
            // 1.111 + 2.222 + 3.333 = 6.666 → 6.67
            var values = new List<double> { 1.111, 2.222, 3.333 };
            double result = ConversionHelper.SumConverted(values);
            Assert.AreEqual(6.67, result);
        }
    }
}