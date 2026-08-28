namespace MyProject.Tests;

public class Calculator {
    public int Add(int a, int b)
    {
        return a + b;
    }
}

[TestFixture]
public class CalculatorTests {
    private Calculator _calculator;

    [SetUp]
    public void Setup() {
        // Выполняется перед каждым тестом
        _calculator = new Calculator();
    }

    [TearDown]
    public void TearDown() {
        // Выполняется после каждого теста
    }

    [Test]
    public void Add_TwoPositiveNumbers_ReturnsSum() {
        // Arrange
        int a = 5, b = 3;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        Assert.That(result, Is.EqualTo(8));
    }

    [TestCase(2, 3, 5)]
    [TestCase(-1, 1, 0)]
    [TestCase(0, 0, 0)]
    public void Add_VariousInputs_ReturnsExpectedSum(int a, int b, int expected) {
        int result = _calculator.Add(a, b);
        Assert.That(result, Is.EqualTo(expected));
    }
}
