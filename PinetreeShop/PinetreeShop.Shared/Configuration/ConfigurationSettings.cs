using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Shared.Configuration
{
    public interface IConfigurationDictionary
    {
        Dictionary<string, string> AppSettings { get; }
    }

    public class ConfigurationDictionary : IConfigurationDictionary
    {
        public Dictionary<string, string> AppSettings
        {
            get; private set;
        }

        public ConfigurationDictionary()
        {
            AppSettings = ConfigurationManager
                .AppSettings
                .AllKeys
                .ToDictionary(
                    x => x,
                    y => ConfigurationManager.AppSettings[y]
                );
        }
    }
}
