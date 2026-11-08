using Everkin.Core;

namespace Everkin.Core.Tests;

public class SetupTests
{
    [Fact]
    public void CoreIsReferenced() => Assert.Equal("Everkin.Core", CoreInfo.Name);
}
