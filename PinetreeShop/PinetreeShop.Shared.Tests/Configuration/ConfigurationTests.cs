using PinetreeShop.Shared.Configuration;
using PinetreeShop.Shared.ConfigurationTests;
using System.Configuration;
using Xunit;

namespace PinetreeShop.Shared.Tests
{
    public class Configuration
    {
        [Fact]
        public void ThrowException_When_SettingNotFoundAndNoDefault()
        {
            var ex = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                new MissingValueConfig(new ConfigurationDictionary());
            });
        }

        [Fact]
        public void ThrowsException_When_SettingNotParsalbe()
        {
            var ex = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                new UnparsableValueConfig(new ConfigurationDictionary());
            });

        }

        [Fact]
        public void SetsValues()
        {
            var config = new WorkingConfig(new ConfigurationDictionary());

            Assert.Equal(config.MissingIntValueWithDefault, 2);
            Assert.Equal(config.OverridenIntValue, 1);
            Assert.Equal(config.IntValue, 1);
            Assert.Equal(config.TrueValue, true);
            Assert.Equal(config.FalseValue, false);
            Assert.Equal(config.StringValue, "some string");
        }
    }
}
