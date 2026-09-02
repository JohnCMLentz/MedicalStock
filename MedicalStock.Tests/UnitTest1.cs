namespace MedicalStock.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Sum_ShouldReturnCorrectResult()
        {
            int number1 = 10;
            int number2 = 20;

            int result = number1 + number2;

            Assert.Equal(30, result);
        }
    }
}
