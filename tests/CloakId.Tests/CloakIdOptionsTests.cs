using Xunit;

namespace CloakId.Tests;

public class CloakIdOptionsTests
{
    [Fact]
    public void MinLength_DefaultValue_ShouldBeZero()
    {
        // Arrange & Act
        var options = new CloakIdOptions();

        // Assert
        Assert.Equal(0, options.MinLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void MinLength_ValidValues_ShouldSetSuccessfully(int validMinLength)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        options.MinLength = validMinLength;
        Assert.Equal(validMinLength, options.MinLength);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void MinLength_NegativeValues_ShouldThrowArgumentOutOfRangeException(int invalidMinLength)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => options.MinLength = invalidMinLength);
        Assert.Equal("MinLength must be 0 or greater. (Parameter 'value')", exception.Message);
    }

    [Fact]
    public void Alphabet_DefaultValue_ShouldBeNull()
    {
        // Arrange & Act
        var options = new CloakIdOptions();

        // Assert
        Assert.Null(options.Alphabet);
    }

    [Fact]
    public void Alphabet_SetToNull_ShouldSucceed()
    {
        // Arrange
        var options = new CloakIdOptions { Alphabet = "abc" };

        // Act
        options.Alphabet = null;

        // Assert
        Assert.Null(options.Alphabet);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")]
    [InlineData("!@#$%^&*()")]
    public void Alphabet_ValidAlphabets_ShouldSetSuccessfully(string validAlphabet)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        options.Alphabet = validAlphabet;
        Assert.Equal(validAlphabet, options.Alphabet);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Alphabet_NullEmptyOrWhitespace_ShouldThrowArgumentException(string invalidAlphabet)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => options.Alphabet = invalidAlphabet);
        Assert.Contains("Alphabet cannot be null, empty, or whitespace", exception.Message);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void Alphabet_LessThanThreeCharacters_ShouldThrowArgumentException(string shortAlphabet)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => options.Alphabet = shortAlphabet);
        Assert.Contains("Alphabet must contain at least 3 characters", exception.Message);
    }

    [Theory]
    [InlineData("aab")]
    [InlineData("abca")]
    [InlineData("AAA")]
    [InlineData("123321")]
    public void Alphabet_WithDuplicateCharacters_ShouldThrowArgumentException(string duplicateAlphabet)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => options.Alphabet = duplicateAlphabet);
        Assert.Contains("Alphabet contains duplicate character", exception.Message);
    }

    [Theory]
    [InlineData("ab c")]
    [InlineData("abc\t")]
    [InlineData("ab\nc")]
    [InlineData("a\rb\nc")]
    public void Alphabet_WithWhitespaceCharacters_ShouldThrowArgumentException(string whitespaceAlphabet)
    {
        // Arrange
        var options = new CloakIdOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => options.Alphabet = whitespaceAlphabet);
        Assert.Contains("Alphabet contains whitespace character", exception.Message);
    }

    [Fact]
    public void Alphabet_SquidsDefaultAlphabet_ShouldSetSuccessfully()
    {
        // Arrange
        var options = new CloakIdOptions();
        const string sqidsDefaultAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Act & Assert
        options.Alphabet = sqidsDefaultAlphabet;
        Assert.Equal(sqidsDefaultAlphabet, options.Alphabet);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new CloakIdOptions();

        // Assert
        Assert.Equal(0, options.MinLength);
        Assert.Null(options.Alphabet);
    }

    [Fact]
    public void Properties_CanBeSetInObjectInitializer()
    {
        // Arrange
        const string testAlphabet = "abcdefghijklmnopqrstuvwxyz";
        const int testMinLength = 5;

        // Act
        var options = new CloakIdOptions
        {
            MinLength = testMinLength,
            Alphabet = testAlphabet
        };

        // Assert
        Assert.Equal(testMinLength, options.MinLength);
        Assert.Equal(testAlphabet, options.Alphabet);
    }
}
