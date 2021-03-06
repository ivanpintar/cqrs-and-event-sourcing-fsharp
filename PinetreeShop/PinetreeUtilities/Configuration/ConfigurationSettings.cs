﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PinetreeUtilities.Configuration
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
