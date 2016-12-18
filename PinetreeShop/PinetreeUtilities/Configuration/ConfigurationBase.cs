using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;

namespace PinetreeUtilities.Configuration
{
    public abstract class ConfigurationBase
    {
        private static Type intType = typeof(long);
        private static Type dateType = typeof(DateTime);
        private static Type boolType = typeof(bool);
        private static Type decimalType = typeof(decimal);
        private readonly IConfigurationDictionary _configSettings;

        protected virtual string Namespace { get { return ""; } }

        public ConfigurationBase(IConfigurationDictionary configSettings)
        {
            _configSettings = configSettings;
            var properties = this.GetType().GetProperties(BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var p in properties)
            {
                UpdateProperty(p);
            }
        }
        private void UpdateProperty(PropertyInfo p)
        {
            var name = Namespace + p.Name;
            var value = GetConfigValue(name);

            if (value == null)
            {
                var defaultVal = p.GetCustomAttribute<DefaultSettingValueAttribute>();
                if (defaultVal != null)
                {
                    value = defaultVal.Value;
                }
            }

            var configVal = ParseConfigValue(value, p.PropertyType);

            if (configVal != null)
            {
                p.SetValue(this, configVal);
                return;
            }


            throw new ConfigurationErrorsException($"Configuration setting {name} is not defined and has no default value");
        }

        private object ParseConfigValue(string value, Type propertyType)
        {
            try
            {
                if (intType == propertyType)
                {
                    return long.Parse(value);
                }
                else if (propertyType == dateType)
                {
                    return DateTime.Parse(value);
                }
                else if (propertyType == boolType)
                {
                    return bool.Parse(value);
                }
                else if (propertyType == decimalType)
                {
                    return decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                else
                {
                    return value;
                }
            }
            catch
            {
                throw new ConfigurationErrorsException($"Error converting setting {value} to {propertyType}");
            }
        }

        private string GetConfigValue(string name)
        {
            if (_configSettings.AppSettings.ContainsKey(name))
                return _configSettings.AppSettings[name];

            return null;
        }
    }
}
