using Xunit;

namespace OniFriendlyFlydos.Tests;

public sealed class CheckboxPolicyTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    public void UsesPlibStateBeforeClick(int currentState, bool expected)
    {
        Assert.Equal(expected, CheckboxPolicy.GetValueAfterClick(currentState));
    }
}
