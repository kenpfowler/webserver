namespace Server.Tests;
using Lib;
using FluentAssertions;

public class FizzBuzzTests
{
    [Fact]
    public void GivenANumberNotDivisibleBy3or5_ThenReturnsNumberAsString()
    {

        int result = FizzBuzzGame.Play(1);
        result.Should().Be(1);

    }
}

