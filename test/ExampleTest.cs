using Xunit;

namespace test
{
    public class ExampleTest
    {
        [Fact]
        public void Add_TwoPlusTwo_Returns4() 
        {
            // Arrange
            int a = 2;
            int b = 2;
            int expected = 4;

            // Act
            int actual = Add(a, b);

            // Assert
            Assert.Equal(expected, actual);
        }

        private int Add(int a, int b) => a + b;
    }
}
