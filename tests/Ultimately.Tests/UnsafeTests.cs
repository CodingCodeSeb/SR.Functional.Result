namespace Ultimately.Tests
{
    using Xunit;

    using Unsafe;


    public class UnsafeTests
    {
        [Fact]
        public void Either_ToNullable()
        {
            Assert.Equal(default, Optional.None<int>("").ToNullable());
            Assert.Equal(1, Optional.Some(1).ToNullable());
        }

        [Fact]
        public void Either_GetValueOrDefault()
        {
            Assert.Equal(default, Optional.None<int>("").ValueOrDefault());
            Assert.Equal(1, Optional.Some(1).ValueOrDefault());

            Assert.Equal(default, Optional.None<int?>("").ValueOrDefault());
            Assert.Equal(1, Optional.Some<int?>(1).ValueOrDefault());

            Assert.Equal(default, Optional.None<string>("").ValueOrDefault());
            Assert.Equal("a", Optional.Some("a").ValueOrDefault());
        }

        [Fact]
        public void Either_GetUnsafeValue()
        {
            var none = Optional.None<string>("ex");
            var some = "a".Some();

            Assert.Equal("a", some.ValueOrFailure());
            Assert.Equal("a", some.ValueOrFailure("Error message"));

            var exception = Assert.Throws<OptionValueMissingException>(() => none.ValueOrFailure("Error message"));

            Assert.Equal("Error message", exception.Message);
        }
    }
}
