using PinetreeShop.Shared.Configuration;
using System.Configuration;

namespace PinetreeShop.Shared.ConfigurationTests
{
    public class MissingValueConfig : ConfigurationBase
    {
        protected override string Namespace { get { return "ConfigTest_"; } }

        public long MissingValue { get; private set;  }

        public MissingValueConfig(IConfigurationDictionary configSettings) : base(configSettings)
        {
        }
    }

    public class UnparsableValueConfig : ConfigurationBase
    {
        protected override string Namespace { get { return "ConfigTest_"; } }

        public long FailingIntValue { get; private set; }

        public UnparsableValueConfig(IConfigurationDictionary configSettings) : base(configSettings)
        {
        }
    }

    public class WorkingConfig : ConfigurationBase
    {
        protected override string Namespace { get { return "ConfigTest_"; } }

        [DefaultSettingValue("2")]
        public long MissingIntValueWithDefault { get; private set; }

        [DefaultSettingValue("2")]
        public long IntWithDefaultTwo { get; private set; }

        public long IntValue { get; private set; }
        public decimal DecimalValue { get; private set; }

        public bool TrueValue { get; private set; }
        public bool FalseValue { get; private set; }
        public string StringValue { get; private set; }

        public WorkingConfig(IConfigurationDictionary configSettings) : base(configSettings)
        {

        }
    }
}
