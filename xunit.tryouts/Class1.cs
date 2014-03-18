using System;
using Xunit;

namespace xunit.tryouts
{
    public class Class1
    {
        [Fact]
        public void CanSkipDynamically()
        {
            throw new SkipException("Dynamic");
        }
    }
}
